import { host } from "./config.js";
const endpoint = "/api/course/access";

const jwt = localStorage.getItem("JWT");

const config = {
  headers: {
    Authorization: "Bearer " + jwt,
  },
};

const urlParams = new URLSearchParams(window.location.search);
const roomId = urlParams.get("id");

axios
  .get(`${host}${endpoint}?id=${roomId}`, config)
  .then((response) => {
    if (response.status === 200) {
      initializeOnlineCourse();
    } else {
      location.href = `${host}/error.html`;
    }
  })
  .catch((err) => {
    location.href = `${host}/error.html`;
    console.log(err);
  });

/******************************
初始化
*******************************/
function initializeOnlineCourse() {
  const urlParams = new URLSearchParams(window.location.search);
  const roomId = urlParams.get("id");
  const title = $(".roomId-container")[0];
  title.innerHTML = `<h5>房間ID: ${roomId}</h5>`;

  const Peers = {};
  let userId = null;
  let localStream = null;

  // 建立signalR連線
  const connection = new signalR.HubConnectionBuilder()
    .withUrl("/videoHub")
    .build();

  // PeerJs
  const myPeer = new Peer();

  // 端對端的連線
  myPeer.on("open", (id) => {
    userId = id;
    const startSignalR = async () => {
      await connection.start();
      await connection.invoke("JoinRoom", roomId, userId);
    };
    startSignalR();
  });

  // 打開鏡頭
  const videoGrid = document.querySelector(".video-grid");
  const myVideo = document.createElement("video");
  myVideo.muted = true;
  myVideo.className = "my-video";

  navigator.mediaDevices
    .getUserMedia({
      audio: true,
      video: true,
    })
    .then((stream) => {
      addVideoStream(myVideo, stream);
      localStream = stream;
    });

  // signalR 偵測到有使用者連上
  connection.on("user-connected", (id) => {
    if (userId === id) return;
    console.log(`User connected : ${id}`);
    connectNewUser(id, localStream);
    myVideo.className = "local-video";
  });

  // signalR 偵測到有使用者離線
  connection.on("user-disconnected", (id) => {
    console.log(`User disconnected : ${id}`);
    myVideo.className = "my-video";
    if (Peers[id]) {
      Peers[id].close();
    }
  });

  // Peerjs 偵測到有連線並有資料傳入
  myPeer.on("call", (call) => {
    call.answer(localStream);

    const userVideo = document.createElement("video");
    userVideo.className = "remote-video";
    call.on("stream", (userVideoStream) => {
      addVideoStream(userVideo, userVideoStream);
      myVideo.className = "local-video";
    });
  });

  /******************************
  Functions
  *******************************/

  // Function 建立視訊流並傳送
  const addVideoStream = (video, stream) => {
    video.srcObject = stream;
    video.addEventListener("loadedmetadata", () => {
      video.play();
    });
    videoGrid.appendChild(video);
  };

  // Function 連接新用戶
  const connectNewUser = (userId, localStream) => {
    const userVideo = document.createElement("video");
    userVideo.className = "remote-video";
    const call = myPeer.call(userId, localStream);

    call.on("stream", (userVideoStream) => {
      addVideoStream(userVideo, userVideoStream);
    });

    call.on("close", () => {
      userVideo.remove();
    });

    Peers[userId] = call;
  };

  /******************************
  攝影機開啟與否
  *******************************/

  const cameraBtn = document.getElementById("toggleCam");
  cameraBtn.textContent = "關閉鏡頭";
  let camEnabled = true; // 初始設定為開啟鏡頭

  cameraBtn.addEventListener("click", () => {
    if (camEnabled && localStream) {
      // 禁用視頻軌道
      localStream.getVideoTracks().forEach((track) => {
        track.enabled = false;
      });
      camEnabled = false;
      cameraBtn.textContent = "開啟鏡頭";
    } else if (!camEnabled && localStream) {
      // 啟用視頻軌道
      localStream.getVideoTracks().forEach((track) => {
        track.enabled = true;
      });
      camEnabled = true;
      cameraBtn.textContent = "關閉鏡頭";
    }
  });

  /******************************
  麥克風開啟與否
  *******************************/

  const micBtn = document.getElementById("toggleMic");
  micBtn.textContent = "關閉麥克風";
  let micEnabled = true; // 初始設定為開啟麥克風

  micBtn.addEventListener("click", () => {
    if (micEnabled && localStream) {
      // 禁用音頻軌道
      localStream.getAudioTracks().forEach((track) => {
        track.enabled = false;
      });
      micEnabled = false;
      micBtn.textContent = "開啟麥克風";
    } else if (!micEnabled && localStream) {
      // 啟用音頻軌道
      localStream.getAudioTracks().forEach((track) => {
        track.enabled = true;
      });
      micEnabled = true;
      micBtn.textContent = "關閉麥克風";
    }
  });

  /******************************
  文字聊天功能
  *******************************/

  // 禁止發送文字訊息，直到建立連接
  $(".send-btn").hide();

  // 連接服務
  const chatConn = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

  // 建立連接
  chatConn
    .start()
    .then(function () {
      // 連接成功顯示發送按鈕
      $(".send-btn").show();
    })
    .catch(function (err) {
      // 連接失敗返回錯誤訊息
      return console.error(err.toString());
    });

  // 發送消息
  $(".send-btn").click(sendMessage);
  $("#chat-input").keyup(function (event) {
    if (event.key === "Enter") {
      sendMessage();
    }
  });

  function sendMessage() {
    const user = localStorage.getItem("userName");
    const message = $("#chat-input").text();
    if (message !== null && message.length > 0) {
      chatConn.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
      });
      $("#chat-input").text("");
    }
  }

  // 接收消息
  chatConn.on("ReceiveMessage", function (user, message, time) {
    $("#chat-content").append(`<p>${user} : ${message}</p>`);
    $("#chat-content").animate({ scrollTop: 100000 });
  });
}
