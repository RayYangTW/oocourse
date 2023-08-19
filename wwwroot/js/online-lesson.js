// ========== Open camera function ==========
// constraints
const videoConstraints = {
  audio: false,
  video: true,
};

// success
function handleVideoSuccess(stream) {
  const video = $("#myVideo")[0];
  video.srcObject = stream;
}

// fail
function handleVideoError(err) {
  console.error("getUserMedia error: " + err);
}

// to open camera
function onOpenCamera(e) {
  navigator.mediaDevices
    .getUserMedia(videoConstraints)
    .then(handleVideoSuccess)
    .catch(handleVideoError);
}

$("#showVideo").on("click", onOpenCamera);

// ========== Open Mic function ==========
// constraints
const micConstraints = {
  audio: true,
  video: false,
};

// success
function handleMicSuccess(stream) {
  const audio = $("#myAudio")[0];
  audio.srcObject = stream;
}

// fail
function handleMicError(err) {
  console.error("getUserMedia error: " + err);
}

// to open camera
function onOpenMic(e) {
  navigator.mediaDevices
    .getUserMedia(micConstraints)
    .then(handleMicSuccess)
    .catch(handleMicError);
}

$("#playAudio").on("click", onOpenMic);

// === Chat feature with SignalR ===
// 連接服務
const connection = new signalR.HubConnectionBuilder()
  .withUrl("/chatHub")
  .build();

// 禁止發送，直到建立連接
$("#sendButton").hide();

// 建立連接
connection
  .start()
  .then(function () {
    // 連接成功顯示發送按鈕
    $("#sendButton").show();
  })
  .catch(function (err) {
    // 連接失敗返回錯誤訊息
    return console.error(err.toString());
  });

// 發送消息
$("#sendButton").click(function () {
  const user = "xxx";
  const message = $("#messageInput").text();
  connection.invoke("SendMessage", user, message).catch(function (err) {
    return console.error(err.toString());
  });
});

// 接收消息
connection.on("ReceiveMessage", function (user, message, time) {
  $("#content").append(`<p>${user} ${time} : ${message}</p><br>`);
  $("#content").animate({ scrollTop: 100000 });
});

// === VideoChat feature with SignalR ===
// 連接服務
const connection2 = new signalR.HubConnectionBuilder()
  .withUrl("/videoChatHub")
  .build();

connection2
  .start()
  .then(() => {
    console.log("okok");
  })
  .catch((err) => {
    console.error(err);
  });

const sendOffer = function (connectionId, sdpOffer, username) {
  signalRService.connection2
    .invoke("Offer", connectionId, sdpOffer, username)
    .catch((error) => console.error("Error sending offer:", error));
};

const sendAnswer = function (connectionId, sdpAnswer, username) {
  signalRService.connection2
    .invoke("ReceiveAnswer", connectionId, sdpAnswer, username)
    .catch((error) => console.error("Error sending answer:", error));
};

const sendIceCandidate = function (connectionId, candidate) {
  signalRService.connection2
    .invoke("SendIceCandidate", connectionId, candidate)
    .catch((error) => console.error("Error sending ICE candidate:", error));
};
