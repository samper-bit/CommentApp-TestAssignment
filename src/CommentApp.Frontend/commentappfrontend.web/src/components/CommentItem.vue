<template>
  <div class="comment-item">
    <!-- View -->
    <div v-if="editingCommentId !== comment.id">
      <div class="comment-header">
        <div class="user-info">
          <strong>{{ comment.userName }}</strong>
          <span class="date">{{ formatDate(comment.createdAt) }}</span>
        </div>
        <div class="comment-actions">
          <button v-if="!isFormVisible" class="reply-button" @click="toggleFormVisibility">Reply</button>
          <button v-if="showDeleteButton && loadedReplies.length < 1" class="delete-button" @click="deleteComment(comment.id)">🗑️</button>
          <button class="edit-button" @click="editComment(comment.id)">✏️</button>
        </div>
      </div>
      <div class="comment-text-file">
        <p class="comment-text" v-html="formatCommentText(comment.text)"></p>
        <div v-if="comment.file">
          <img v-if="isImage(comment.file.filePath)"
               :src="`https://localhost:${apiPort}/files/${comment.file.filePath}`"
               class="comment-file" />
          <div v-if="isTextFile(comment.file.filePath)">
            <a :href="`https://localhost:${apiPort}/files/${comment.file.filePath}`" target="_blank">Download Text File</a>
          </div>
        </div>
      </div>
    </div>

    <!-- Edit -->
    <div v-else class="edit-comment">
      <div class="tag-panel">
        <button type="button" class="tag-button" @click="insertTag('i', $refs.editTextArea, editedText, 'edit')">[i]</button>
        <button type="button" class="tag-button" @click="insertTag('strong', $refs.editTextArea, editedText, 'edit')">[strong]</button>
        <button type="button" class="tag-button" @click="insertTag('code', $refs.editTextArea, editedText, 'edit')">[code]</button>
        <button type="button" class="tag-button" @click="insertTag('a', $refs.editTextArea, editedText, 'edit')">[a]</button>
      </div>

      <textarea v-model="editedText" ref="editTextArea" required></textarea>
      <span v-if="errors.text" class="error">{{ errors.text }}</span>

      <div v-if="comment.file && editedFile">
        <img v-if="isImage(comment.file.filePath)"
             :src="`https://localhost:${apiPort}/files/${comment.file.filePath}`"
             class="comment-file" />
        <div v-if="isTextFile(comment.file.filePath)">
          <a :href="`https://localhost:${apiPort}/files/${comment.file.filePath}`" target="_blank">Text File</a>
        </div>
        <div>
          <button @click="removeFile">❌</button>
        </div>
      </div>

      <input type="file" @change="handleEditedFileUpload">
      <span v-if="errors.file" class="error">{{ errors.file }}</span>
      <div class="button-container">
        <button class="update-button" @click="updateComment(comment.id)">Update</button>
        <button class="cancel-button" @click="cancelEditing()">Cancel</button>
      </div>
    </div>

    <!-- Reply Form -->
    <div>
      <div v-if="isFormVisible" class="comment-form">
        <form @submit.prevent="submitForm">
          <div>
            <label for="userName">User Name</label>
            <input type="text" id="userName" v-model="formData.userName" required />
            <span v-if="errors.userName" class="error">{{ errors.userName }}</span>
          </div>

          <div>
            <label for="email">Email</label>
            <input type="email" id="email" v-model="formData.email" required />
            <span v-if="errors.email" class="error">{{ errors.email }}</span>
          </div>

          <div>
            <label for="homePage">Home Page (optional)</label>
            <input type="url" id="homePage" v-model="formData.homePage" />
            <span v-if="errors.homePage" class="error">{{ errors.homePage }}</span>
          </div>

          <div>
            <label for="captcha">Captcha</label>
            <img :src="captchaImageUrl" alt="Captcha" />
            <input type="text" v-model="formData.captchaInput" required />
            <span v-if="errors.captcha" class="error">{{ errors.captcha }}</span>
          </div>

          <div>
            <label for="text">Text</label>
            <div class="tag-panel">
              <button type="button" class="tag-button" @click="insertTag('i', $refs.replyTextArea, formData.text, 'form')">[i]</button>
              <button type="button" class="tag-button" @click="insertTag('strong', $refs.replyTextArea, formData.text, 'form')">[strong]</button>
              <button type="button" class="tag-button" @click="insertTag('code', $refs.replyTextArea, formData.text, 'form')">[code]</button>
              <button type="button" class="tag-button" @click="insertTag('a', $refs.replyTextArea, formData.text, 'form')">[a]</button>
            </div>
            <textarea id="text" v-model="formData.text" ref="replyTextArea" required></textarea>
            <span v-if="errors.text" class="error">{{ errors.text }}</span>
          </div>

          <div>
            <label for="file">Attach File</label>
            <input type="file" id="file" @change="handleFileUpload" />
            <span v-if="errors.file" class="error">{{ errors.file }}</span>
          </div>

          <div>
            <button type="submit" class="send-button">Send</button>
            <button type="button" class="cancel-button" @click="cancelReplying()">Cancel</button>
          </div>
        </form>
      </div>
    </div>

    <!-- Child Comments -->
    <div class="child-comments" v-if="loadedReplies && loadedReplies.length">
      <CommentItem v-for="child in loadedReplies"
                   :key="child.id"
                   :comment="child"
                   @delete-comment="reloadReplies" />
    </div>
    <button v-if="loadRepliesVisible !== false" class="show-replies-button" @click="loadReplies">
      Load More Replies
    </button>
  </div>
</template>

<script setup>
  import { inject, onMounted, ref, reactive, defineEmits } from 'vue'

  const props = defineProps({
    comment: {
      type: Object,
      required: true,
      default: () => ({}),
      validator: (value) => {
        return (
          typeof value.id === "string" &&
          typeof value.userName === "string" &&
          typeof value.email === "string" &&
          (value.homePage === null || typeof value.homePage === "string") &&
          typeof value.text === "string" &&
          (value.parentCommentId === null || typeof value.parentCommentId === "string") &&
          (value.file === null ||
            (typeof value.file === "object" &&
              ("filePath" in value.file ? typeof value.file.filePath === "string" : true))) &&
          (value.childComments === undefined || Array.isArray(value.childComments))
        );
      },
    },
  });
  const apiPort = import.meta.env.VITE_API_PORT;
  const pageIndex = ref(0);
  const showDeleteButton = ref(false);
  const isEditing = ref(false);
  const editedText = ref(props.comment.text);
  const editedFile = ref(null);
  const editingCommentId = ref(null);
  const loadRepliesVisible = ref(false);
  const loadedReplies = ref(props.comment.childComments ? [...props.comment.childComments] : []);
  const isFormVisible = ref(false);
  const formData = reactive({
    parentCommentId: props.comment.id,
    userName: "",
    email: "",
    homePage: "",
    captchaInput: "",
    text: "",
    file: null,
  });
  const errors = reactive({});
  const captchaId = ref(null);
  const captchaImageUrl = ref("");
  const replyTextArea = ref(null);
  const editTextArea = ref(null);
  const emits = defineEmits(["delete-comment"]);

  const formatDate = (date) => {
    const d = new Date(date);
    const day = String(d.getDate()).padStart(2, "0");
    const month = String(d.getMonth() + 1).padStart(2, "0");
    const year = String(d.getFullYear()).slice(2);
    const hours = String(d.getHours()).padStart(2, "0");
    const minutes = String(d.getMinutes()).padStart(2, "0");
    return `${day}.${month}.${year} в ${hours}:${minutes}`;
  };
  const formatCommentText = (text) => {
    if (!text) return "";

    let formatted = text
      .replace(/\[i\](.*?)\[\/i\]/gi, "<i>$1</i>")
      .replace(/\[strong\](.*?)\[\/strong\]/gi, "<strong>$1</strong>")
      .replace(/\[code\](.*?)\[\/code\]/gi, "<code>$1</code>")
      .replace(/\[a\](.*?)\[\/a\]/gi, '<a href="$1" target="_blank">$1</a>');

    return formatted;
  };
  const isImage = (filePath) => {
    const imageExtensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp"];
    return imageExtensions.some((ext) => filePath.endsWith(ext));
  };
  const isTextFile = (filePath) => filePath.endsWith(".txt");
  const openImage = (imagePath) => {
    currentImage.value = imagePath;
    isImageModalVisible.value = true;
  };
  const closeImageModal = () => {
    isImageModalVisible.value = false;
    currentImage.value = "";
  };
  const toggleFormVisibility = () => {
    isFormVisible.value = !isFormVisible.value;
    if (isFormVisible.value) {
      fetchCaptcha();
    }
  };
  const fetchCaptcha = async () => {
    try {
      const response = await fetch(`https://localhost:${apiPort}/captcha`);
      if (response.ok) {
        const data = await response.json();
        captchaId.value = data.captchaId;
        captchaImageUrl.value = data.imageUrl;
      } else {
        console.error("Failed to fetch captcha");
      }
    } catch (error) {
      console.error("Error fetching captcha:", error);
    }
  };
  const verifyCaptcha = async (captchaInput) => {
    try {
      const response = await fetch(`https://localhost:${apiPort}/verify-captcha`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ captchaId: captchaId.value, userInput: captchaInput }),
      });

      const result = await response.json();
      return result.isValid;
    } catch (error) {
      console.error("Error verifying captcha:", error);
      return false;
    }
  };
  const handleEditedFileUpload = (event) => {
    editedFile.value = event.target.files[0];
  };
  const removeFile = () => {
    editedFile.value = null;
  };
  const editComment = async () => {
    if (props.comment) {
      editingCommentId.value = props.comment.id;
      editedText.value = props.comment.text;
      editedFile.value = props.comment.file;

      if (props.comment.file) {
        try {
          const response = await fetch(`https://localhost:${apiPort}/files/${props.comment.file.filePath}`);
          if (!response.ok) throw new Error("Failed to fetch file");

          const blob = await response.blob();
          const fileName = props.comment.file.filePath.split("/").pop();
          editedFile.value = new File([blob], fileName, { type: blob.type });
        } catch (error) {
          console.error("Error loading file:", error);
        }
      }
    }
  };
  const updateComment = async (commentId) => {
    Object.keys(errors).forEach(key => {
      delete errors[key];
    });

    try {
      if (!editedText.value) {
        errors.text = "Text cannot be empty";
      }
      if (editedFile.value) {
        const allowedTypes = ["image/jpeg", "image/png", "image/gif", "text/plain"];

        if (!allowedTypes.includes(editedFile.value.type)) {
          errors.file = "Invalid file type";
        } else if (editedFile.value.type === "text/plain" && editedFile.value.size > 100 * 1024) {
          errors.file = "Text files cannot be larger than 100 KB";
        }
      }

      if (Object.keys(errors).length > 0) {
        return;
      }

      const formData = new FormData();
      formData.append("id", commentId);
      formData.append("text", editedText.value);
      if (editedFile.value) {
        formData.append("file", editedFile.value);
      }

      const response = await fetch(`https://localhost:${apiPort}/comments`, {
        method: "PUT",
        body: formData,
      });

      if (!response.ok) {
        throw new Error("Failed to update comment");
      }

      editingCommentId.value = null;
      editedText.value = "";
      editedFile.value = null;
    } catch (error) {
      console.error("Error updating comment:", error);
    }
  };
  const cancelEditing = () => {
    editingCommentId.value = null;
    editedText.value = "";
    editedFile.value = null;
  };
  const deleteComment = async (commentId) => {
    if (!confirm("Are you sure you want to delete this comment?")) {
      return;
    }

    try {
      const response = await fetch(`https://localhost:${apiPort}/comments/${commentId}`, {
        method: "DELETE",
      });

      if (!response.ok) {
        throw new Error("Failed to delete comment");
      }

      emits("delete-comment");
    } catch (error) {
      console.error("Error deleting comment:", error);
    }
  };
  const loadReplies = async () => {
    const pageSize = 3;
    const url = `https://localhost:${apiPort}/comments/parent/${props.comment.id}?PageIndex=${pageIndex.value}&PageSize=${pageSize}&SortField=CreatedAt&Ascending=false`;

    try {
      const response = await fetch(url);
      if (!response.ok) {
        console.error(`Failed to load replies: ${response.statusText}`);
        return;
      }

      const data = await response.json();

      if (data.comments.data.length !== 0) {
        loadedReplies.value = loadedReplies.value.concat(data.comments.data);
        pageIndex.value++;
        repliesVisible();
      } else {
        loadRepliesVisible.value = false;
      }
    } catch (error) {
      console.error("Error loading replies:", error);
    }
  };
  const repliesVisible = async () => {
    const pageSize = 3;
    try {
      const response = await fetch(
        `https://localhost:${apiPort}/comments/parent/${props.comment.id}?PageIndex=${pageIndex.value}&PageSize=${pageSize}&SortField=CreatedAt&Ascending=false`
      );
      const data = await response.json();
      loadRepliesVisible.value = data.comments.data.length !== 0;

      if (loadedReplies === 0) {
        pageIndex.value = 0;
      }
    } catch (error) {
      console.error("Error fetching replies:", error);
    }
  };
  const reloadReplies = (comment) => {
    pageIndex.value = 0;
    loadedReplies.value = [];
    repliesVisible(comment);
    loadReplies(comment);
  };
  const cancelReplying = () => {
    formData.value = { userName: "", email: "", homePage: "", captchaInput: "", text: "", file: null };
    errors.value = {};
    isFormVisible.value = false;
  };
  const submitForm = async () => {
    Object.keys(errors).forEach(key => {
      delete errors[key];
    });

    if (!formData.userName.match(/^[a-zA-Z0-9]+$/)) {
      errors.userName = "User name must contain only letters and numbers";
    }
    if (!formData.email.match(/^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/)) {
      errors.email = "Please enter a valid email";
    }
    if (formData.homePage && !formData.homePage.match(/^(http|https):\/\/[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,6}(\/[a-zA-Z0-9#\-\/]*)?$/)) {
      errors.homePage = "Please enter a valid URL";
    }
    if (!formData.text) {
      errors.text = "Text cannot be empty";
    }
    if (formData.file) {
      const allowedTypes = ["image/jpeg", "image/png", "image/gif", "text/plain"];

      if (!allowedTypes.includes(formData.file.type)) {
        errors.file = "Invalid file type";
      } else if (formData.file.type === "text/plain" && formData.file.size > 100 * 1024) {
        errors.file = "Text files cannot be larger than 100 KB";
      }
    }

    if (Object.keys(errors).length > 0) {
      return;
    }

    const isCaptchaValid = await verifyCaptcha(formData.captchaInput);
    if (!isCaptchaValid) {
      errors.captcha = "Invalid captcha";
      return;
    }

    const submissionData = new FormData();
    submissionData.append("parentCommentId", props.comment.id);
    submissionData.append("userName", formData.userName);
    submissionData.append("email", formData.email);
    submissionData.append("homePage", formData.homePage);
    submissionData.append("text", formData.text);
    if (formData.file) {
      submissionData.append("file", formData.file);
    }

    try {
      const response = await fetch(`https://localhost:${apiPort}/comments`, {
        method: "POST",
        body: submissionData,
      });
      if (response.ok) {
        isFormVisible.value = false;
        Object.assign(formData, { userName: "", email: "", homePage: "", captchaInput: "", text: "", file: null });
        pageIndex.value = 0;
        loadedReplies.value = [];
      } else {
        console.error("Failed to submit comment");
      }
    } catch (error) {
      console.error("Error submitting comment:", error);
    }
  };
  const handleFileUpload = (event) => {
    formData.file = event.target.files[0];
  };
  const insertTag = (tag, textarea, text, model) => {
    if (!textarea) {
      return;
    }

    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;

    let openTag = '';
    let closeTag = '';

    switch (tag) {
      case 'i':
        openTag = '[i]';
        closeTag = '[/i]';
        break;
      case 'strong':
        openTag = '[strong]';
        closeTag = '[/strong]';
        break;
      case 'code':
        openTag = '[code]';
        closeTag = '[/code]';
        break;
      case 'a':
        openTag = '[a]';
        closeTag = '[/a]';
        break;
      default:
        break;
    }
    const selectedText = text.substring(start, end);
    const newText = text.substring(0, start) + openTag + selectedText + closeTag + text.substring(end);

    if (model === 'form') {
      formData.text = newText;
    } else if (model === 'edit') {
      editedText.value = newText;
    }

    textarea.focus();
    const cursorPos = start + openTag.length + (selectedText ? selectedText.length : 0);
    textarea.setSelectionRange(cursorPos, cursorPos);
  };
  const isShowDeleteButton = async () => {
    const pageSize = 3;
    try {
      const response = await fetch(
        `https://localhost:${apiPort}/comments/parent/${props.comment.id}?PageIndex=${pageIndex.value}&PageSize=${pageSize}&SortField=CreatedAt&Ascending=false`
      );
      const data = await response.json();
      return data.comments.data.length === 0;
    } catch (error) {
      console.error("Error fetching delete button state:", error);
      return false;
    }
  };
  const fetchCommentFile = async (filePath) => {
    if (filePath) {
      try {
        const response = await fetch(`https://localhost:${apiPort}/files/${filePath}`);
        if (!response.ok) throw new Error("Failed to fetch file");

        const blob = await response.blob();
        const fileName = props.comment.file.filePath.split("/").pop();
        return new File([blob], fileName, { type: blob.type });
      } catch (error) {
        console.error("Error loading file:", error);
      }
    }
  }
  const signalRConnection = inject("signalRConnection")
  onMounted(async () => {
    await repliesVisible();
    showDeleteButton.value = await isShowDeleteButton();
    const waitForConnection = setInterval(() => {
      if (signalRConnection?.value) {
        signalRConnection.value.on("ReceiveEditedComment", (updatedData) => {
          if (props.comment.id === updatedData.commentId.value) {
            try {
              props.comment.text = updatedData.text
              if (updatedData.file) {
                props.comment.file = Object.assign({}, props.comment.file, {
                  filePath: updatedData.file.filePath,
                });
              }
              else {
                props.comment.file = null
              }
            } catch (error) {
              console.error("Error:", error);
            }
          }
        });

        signalRConnection.value.on("ReceiveNewReply", (newReply) => {
          if (props.comment.id === newReply.parentCommentId.value) {
            reloadReplies();
          }
        });

        clearInterval(waitForConnection);
      }
    }, 100);
  });

</script>

<style scoped>

  .child-comments {
    margin-left: 50px;
    min-width: 800px;
  }

  .comment-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 10px;
  }

  .comment-actions {
    display: flex;
  }

  .delete-button,
  .edit-button {
    background-color: transparent;
    border: none;
    cursor: pointer;
    color: #007bff;
    margin-left: 5px;
  }

    .delete-button:hover,
    .edit-button:hover {
      color: #0056b3;
    }

  .comment-file {
    max-width: 320px;
    max-height: 240px;
    margin-top: 10px;
    object-fit: cover;
    cursor: pointer;
    transition: transform 0.3s ease;
  }

    .comment-file:hover {
      transform: scale(1.3);
    }

  .reply-button {
    right: 10px;
    bottom: 10px;
    background-color: transparent;
    border: none;
    color: #007bff;
    font-size: 14px;
    cursor: pointer;
    padding: 5px 10px;
  }

    .reply-button:hover {
      background-color: transparent;
      color: #007bff;
      border: none;
    }

  .show-replies-button {
    display: inline-block;
    padding: 8px 16px;
    background-color: transparent;
    color: #007bff;
    font-size: 14px;
    border: none;
    cursor: pointer;
    text-align: center;
    margin: 10px auto 0;
    display: block;
  }

    .show-replies-button:before {
      content: "↓";
      font-size: 24px;
      color: #007bff;
      transition: transform 0.3s ease;
    }

    .show-replies-button:hover:before {
      transform: rotate(180deg);
    }

    .show-replies-button:focus {
      outline: none;
    }

  .edit-comment {
    display: flex;
    flex-direction: column;
    gap: 10px;
    background: #f9f9f9;
    padding: 15px;
    border-radius: 8px;
    border: 1px solid #ddd;
  }

    .edit-comment textarea {
      width: 100%;
      min-height: 100px;
      resize: vertical;
      padding: 10px;
      font-size: 16px;
      border: 1px solid #ccc;
      border-radius: 5px;
      background: #fff;
    }

    .edit-comment input[type="file"] {
      padding: 5px;
      font-size: 14px;
    }

    .edit-comment a {
      color: #007bff;
      text-decoration: none;
      font-weight: bold;
    }

      .edit-comment a:hover {
        text-decoration: underline;
      }

    .edit-comment button {
      align-self: flex-start;
      padding: 8px 12px;
      font-size: 14px;
      border: none;
      border-radius: 5px;
      cursor: pointer;
      transition: background 0.2s ease;
    }

      .edit-comment button:hover {
        opacity: 0.8;
      }

  .update-button {
    background: #28a745;
    color: white;
    margin-right: 4px;
  }

  .cancel-button {
    background: red;
    color: white;
  }

  .tag-button {
    background: #0056b3;
    color: white;
    margin-right: 2px;
    margin-bottom: 3px;
  }

  .comment-form {
    margin: 20px auto;
    padding: 20px;
    background: white;
    border-radius: 8px;
    box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);
    margin-left: 30px;
    min-width: 800px;
  }

    .comment-form input,
    .comment-form textarea {
      width: calc(100% - 16px);
      padding: 8px;
      margin-bottom: 10px;
      border: 1px solid #ccc;
      border-radius: 5px;
    }

    .comment-form textarea {
      resize: vertical;
      height: 120px;
    }

    .comment-form img {
      display: block;
      margin: 10px 0;
      border-radius: 5px;
    }

    .comment-form button {
      align-self: flex-start;
      padding: 8px 12px;
      font-size: 14px;
      border: none;
      border-radius: 5px;
      cursor: pointer;
      transition: background 0.2s ease;
    }

      .comment-form button:hover {
        opacity: 0.8;
      }

  .send-button {
    background-color: #28a745;
    color: white;
    border: none;
    padding: 12px;
    margin-right: 3px;
    border-radius: 5px;
    cursor: pointer;
    transition: background 0.3s ease;
  }

    .send-button:hover {
      background-color: #218838;
    }

  .error {
    color: red;
    font-size: 12px;
  }

  .comment-text-file {
    border: 1px solid #000;
    border-radius: 8px;
    padding: 8px;
    background-color: #f9f9f9;
    margin-bottom: 20px;
    display: flex;
    flex-direction: column;
    align-items: flex-start;
  }

  @keyframes fadeIn {
    from {
      opacity: 0;
    }

    to {
      opacity: 1;
    }
  }

  @keyframes zoomIn {
    from {
      transform: scale(0.8);
    }

    to {
      transform: scale(1);
    }
  }
</style>
