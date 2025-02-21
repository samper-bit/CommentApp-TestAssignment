<template>
  <div class="comments-container" @scroll="onScroll">
    <div>
      <div>
        <button v-if="!isFormVisible" class="show-form-button" @click="toggleFormVisibility">Comment</button>
        <div class="sort-container">
          <label for="sortField">Sort By:</label>
          <select id="sortField" v-model="selectedSortField" @change="toggleSortField">
            <option value="UserName">User Name</option>
            <option value="Email">Email</option>
            <option value="CreatedAt">Created At</option>
          </select>

          <button class="sort-button" @click="toggleSortOrder">
            <span v-if="isAscending">▲</span>
            <span v-else>▼</span>
          </button>
        </div>

      </div>
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
              <button type="button" class="tag-button" @click="insertTag('i')">[i]</button>
              <button type="button" class="tag-button" @click="insertTag('strong')">[strong]</button>
              <button type="button" class="tag-button" @click="insertTag('code')">[code]</button>
              <button type="button" class="tag-button" @click="insertTag('a')">[a]</button>
            </div>
            <textarea id="text" v-model="formData.text" ref="textArea" required></textarea>
            <span v-if="errors.text" class="error">{{ errors.text }}</span>
          </div>

          <div>
            <label for="file">Attach File</label>
            <input type="file" id="file" @change="handleFileUpload" />
            <span v-if="errors.file" class="error">{{ errors.file }}</span>
          </div>

          <button type="submit" class="send-button">Send</button>
          <button type="button" class="cancel-button" @click="cancelAddingComment()">Cancel</button>
        </form>
      </div>

    </div>

    <CommentItem v-for="comment in comments"
                 :key="comment.id"
                 :comment="comment"
                 @delete-comment="fetchComments"
                 class="comment" />
  </div>
</template>

<script>
  import CommentItem from './CommentItem.vue';
  import { inject, onMounted, ref, reactive } from 'vue'

  export default {
    components: { CommentItem },
    setup() {
      const comments = ref([]);
      const pageIndex = ref(0);
      const isAscending = ref(false);
      const selectedSortField = ref("CreatedAt");
      const isLoading = ref(false);
      const formData = reactive({
        userName: "",
        email: "",
        homePage: "",
        captchaInput: "",
        text: "",
        file: null,
      });
      const isFormVisible = ref(false);
      const errors = reactive({});
      const captchaId = ref(null);
      const captchaImageUrl = ref("");

      const fetchComments = async () => {
        try {
          if (pageIndex.value === 0) {
            comments.value = [];
          }
          const response = await fetch(`https://localhost:5050/comments/roots?PageIndex=${pageIndex.value}&PageSize=${25}&SortField=${selectedSortField.value}&Ascending=${isAscending.value}`);
          if (response.ok) {
            const data = await response.json();
            comments.value = comments.value.concat(data.comments.data);
          } else {
            console.error("Failed to load comments");
          }
        } catch (error) {
          console.error("Error fetching comments:", error);
        }
      };
      const toggleFormVisibility = () => {
        isFormVisible.value = !isFormVisible.value;
        if (isFormVisible.value) {
          fetchCaptcha();
        }
      };
      const insertTag = (tag) => {
        const textarea = document.querySelector('textarea');
        if (!textarea) {
          return;
        }

        const start = textarea.selectionStart;
        const end = textarea.selectionEnd;

        const text = formData.text;
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
        formData.text = newText;

        textarea.focus();
        const cursorPos = start + openTag.length + (selectedText ? selectedText.length : 0);
        textarea.setSelectionRange(cursorPos, cursorPos);
      };
      const fetchCaptcha = async () => {
        try {
          const response = await fetch('https://localhost:5050/captcha');
          if (response.ok) {
            const data = await response.json();
            captchaId.value = data.captchaId;
            captchaImageUrl.value = data.imageUrl;
          } else {
            console.error('Failed to fetch captcha');
          }
        } catch (error) {
          console.error('Error fetching captcha:', error);
        }
      };
      const verifyCaptcha = async () => {
        const response = await fetch('https://localhost:5050/verify-captcha', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            captchaId: captchaId.value,
            userInput: formData.captchaInput,
          }),
        });
        const result = await response.json();
        return result.isValid;
      };
      const handleFileUpload = (event) => {
        formData.file = event.target.files[0];
      };
      const submitForm = async () => {
        errors.value = {};
        if (!formData.userName.match(/^[a-zA-Z0-9]+$/)) {
          errors.userName = 'User name must contain only letters and numbers';
        }
        if (!formData.email.match(/^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/)) {
          errors.email = 'Please enter a valid email';
        }
        if (formData.homePage && !formData.homePage.match(/^(http|https):\/\/[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,6}(\/[a-zA-Z0-9#\-\/]*)?$/)) {
          errors.homePage = 'Please enter a valid URL';
        }
        if (!formData.text) {
          errors.text = 'Text cannot be empty';
        }
        if (formData.file) {
          const allowedTypes = ['image/jpeg', 'image/png', 'image/gif', 'text/plain'];

          if (!allowedTypes.includes(formData.file.type)) {
            errors.file = 'Invalid file type';
          } else if (formData.file.type === 'text/plain' && formData.file.size > 100 * 1024) {
            errors.file = 'Text files cannot be larger than 100 KB';
          }
        }

        if (Object.keys(errors.value).length > 0) {
          return;
        }

        const isCaptchaValid = await verifyCaptcha();
        if (!isCaptchaValid) {
          errors.captcha = 'Invalid captcha';
          return;
        }

        const formDataToSubmit = new FormData();
        formDataToSubmit.append('userName', formData.userName);
        formDataToSubmit.append('email', formData.email);
        formDataToSubmit.append('homePage', formData.homePage);
        formDataToSubmit.append('text', formData.text);
        if (formData.file) {
          formDataToSubmit.append('file', formData.file);
        }

        try {
          const response = await fetch('https://localhost:5050/comments', {
            method: 'POST',
            body: formDataToSubmit,
          });
          if (response.ok) {
            isFormVisible.value = false;
            Object.assign(formData, { userName: '', email: '', homePage: '', captchaInput: '', text: '', file: null });
          } else {
            console.error('Failed to submit comment');
          }
        } catch (error) {
          console.error('Error submitting comment:', error);
        }
      };
      const cancelAddingComment = () => {
        Object.assign(formData, { userName: '', email: '', homePage: '', captchaInput: '', text: '', file: null });
        errors.value = {};
        isFormVisible.value = false;
      };
      const toggleSortOrder = () => {
        isAscending.value = !isAscending.value;
        pageIndex.value = 0;
        fetchComments();
      };
      const toggleSortField = () => {
        pageIndex.value = 0;
        fetchComments();
      };
      const canLoadRootComments = async () => {
        const pageSize = 1;
        const response = await fetch(`https://localhost:5050/comments/roots?PageIndex=${pageIndex.value + 1}&PageSize=${pageSize}&SortField=${selectedSortField.value}&Ascending=${isAscending.value}`);
        const data = await response.json();
        return data.comments.data.length > 0;
      };
      const onScroll = async (event) => {
        const container = event.target;
        const threshold = 50;

        if (container.scrollHeight - container.scrollTop - container.clientHeight <= threshold) {
          if (!isLoading.value && await canLoadRootComments()) {
            isLoading.value = true;
            pageIndex.value++;
            await fetchComments();
            isLoading.value = false;
          }
        }
      };
      const signalRConnection = inject("signalRConnection")
      onMounted(() => {
        fetchComments();
        const waitForConnection = setInterval(() => {
          if (signalRConnection?.value) {
            signalRConnection.value.on("ReceiveNewComment", () => {
              fetchComments();
            });
            clearInterval(waitForConnection);
          }
        }, 100);
      });
      return {
        comments,
        pageIndex,
        isAscending,
        selectedSortField,
        isLoading,
        formData,
        isFormVisible,
        errors,
        captchaId,
        captchaImageUrl,
        fetchComments,
        toggleFormVisibility,
        insertTag,
        fetchCaptcha,
        verifyCaptcha,
        handleFileUpload,
        submitForm,
        cancelAddingComment,
        toggleSortOrder,
        toggleSortField,
        canLoadRootComments,
        onScroll,
        signalRConnection,
      }
    },
  };
</script>

<style scoped>
  .comments-container {
    max-height: 1000px;
    overflow-y: auto;
    padding-right: 10px;
  }

  .comment {
    position: relative;
    background-color: #f8f9fa;
    border: 1px solid #dcdcdc;
    border-radius: 5px;
    padding: 15px;
    margin-bottom: 15px;
  }

  .show-form-button {
    background-color: #007bff;
    color: white;
    border: none;
    padding: 10px 20px;
    font-size: 16px;
    margin-bottom: 10px;
    border-radius: 5px;
    cursor: pointer;
    transition: background 0.3s ease;
    display: block;
    width: fit-content;
    text-align: left;
    margin-left: 0 !important;
  }

    .show-form-button:hover {
      background-color: #0056b3;
    }

  .comment-form {
    width: 100%;
    margin: 20px auto;
    padding: 20px;
    background: white;
    border-radius: 8px;
    box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);
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

  .cancel-button {
    background: red;
    color: white;
    border: none;
    padding: 12px;
    margin-right: 3px;
    border-radius: 5px;
    cursor: pointer;
    transition: background 0.3s ease;
  }

  .tag-button {
    background: #0056b3;
    color: white;
    margin-right: 2px;
    margin-bottom: 3px;
    border: none;
    padding: 12px;
    border-radius: 5px;
    cursor: pointer;
    transition: background 0.3s ease;
  }

  .error {
    color: red;
    font-size: 12px;
  }

  .sort-container {
    display: flex;
    align-items: center;
    gap: 10px;
    margin-bottom: 15px;
  }

    .sort-container label {
      font-size: 16px;
      font-weight: bold;
    }

    .sort-container select {
      padding: 5px;
      font-size: 14px;
      border: 1px solid #ccc;
      border-radius: 4px;
    }

  .sort-button {
    background: none;
    border: none;
    font-size: 18px;
    cursor: pointer;
    padding: 5px;
  }

    .sort-button:hover {
      color: #007bff;
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
