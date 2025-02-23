<template>
  <div id="app">
    <CommentList />
  </div>
</template>

<script>
  import CommentList from './components/CommentList.vue';
  import CommentItem from './components/CommentItem.vue';
  import { ref, onMounted, onBeforeUnmount, provide } from "vue";
  import * as signalR from "@microsoft/signalr";

  export default {
    name: 'App',
    components: { CommentList, CommentItem },
    setup() {
      const apiPort = import.meta.env.VITE_API_PORT;
      const connection = ref(null);
      const isConnected = ref(false);

      provide("signalRConnection", connection);

      const startSignalRConnection = async () => {
        connection.value = new signalR.HubConnectionBuilder()
          .withUrl(`https://localhost:${apiPort}/chatHub`)
          .build();

        try {
          await connection.value.start();
          console.log("✅ SignalR connection established");
          isConnected.value = true;
        } catch (err) {
          console.error("❌ SignalR connection failed:", err);
        }
      };

      onMounted(async () => {
        await startSignalRConnection();
      });

      onBeforeUnmount(() => {
        if (connection.value) {
          connection.value.stop().catch(err => console.error("❌ Error stopping SignalR connection: ", err));
        }
      });

      return { isConnected };
    }
  };
</script>


<style>
  #app {
    font-family: Avenir, Helvetica, Arial, sans-serif;
    text-align: left;
    color: #2c3e50;
    background-color: #ffffff;
    margin-top: 20px;
    width: 100%;
    display: block;
  }

  .comments-container {
    width: 100%;
    padding: 20px;
    background-color: #fff;
    margin: 0 auto;
  }

  .comment {
    width: 100%;
    padding: 20px;
    margin-bottom: 20px;
    border-radius: 8px;
    background-color: #f9f9f9;
    box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
    display: flex;
    flex-direction: column;
  }

  .comment-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 10px;
  }

  .user-info {
    display: flex;
    flex-direction: column;
  }

    .user-info strong {
      font-size: 16px;
      color: #333;
    }

  .date {
    font-size: 12px;
    color: #888;
  }

  .comment-actions {
    display: flex;
    gap: 10px;
  }

    .comment-actions button {
      background: none;
      border: none;
      cursor: pointer;
      font-size: 18px;
    }

    .comment-actions .delete-button {
      color: #dc3545;
    }

    .comment-actions .edit-button {
      color: #ffc107;
    }

  .comment-text {
    font-size: 16px;
    line-height: 1.6;
    margin: 15px 0;
  }

  .comment .reply-button {
    background-color: transparent;
    border: 1px solid #007bff;
    color: #007bff;
    padding: 5px 10px;
    font-size: 12px;
    cursor: pointer;
  }

    .comment .reply-button:hover {
      background-color: #007bff;
      color: white;
    }

  @media (max-width: 1024px) {
    .comments-container {
      padding: 10px;
    }

    .comment {
      padding: 15px;
      margin-bottom: 15px;
    }
  }
</style>
