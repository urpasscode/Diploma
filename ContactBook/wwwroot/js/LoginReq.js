async function PostUserLogin() {
  const response = await fetch("/User/Login", {
    method: "POST",
    body: new FormData(document.forms.loginForm),
  });
  console.log(response.status);
  if (response.ok) {
    const user = await response.json();
    console.log(user);
    window.location.href = `https://localhost:7148/Main`;
  } else {
    const errorData = await response.json();
    if (errorData) {
      // Выводим ошибки пользователю
      document.getElementById("message_log").innerHTML = errorData + "<br/>";
      // Выводим ошибки в консоль
      console.error("Ошибки от сервера:", errorData);
    } else {
      alert("Ошибка сервера. Попробуйте позже.");
    }
  }
}
async function PostUserSignup() {
  const response1 = await fetch("/User/Signup", {
    method: "POST",
    body: new FormData(document.forms.registerForm),
  });
  console.log(response1.status);
  if (response1.ok) {
    const user = await response1.json();
    console.log(user);
    window.location.href = `https://localhost:7148/Main`;
  } else {
    const errorData = await response1.json();
    if (errorData) {
      // Выводим ошибки в консоль
      console.error("Ошибки от сервера:", errorData);
      // Выводим ошибки пользователю
      document.getElementById("message_reg").innerHTML = errorData + "<br/>";
    } else {
      alert("Ошибка сервера. Попробуйте позже.");
    }
  }
}
