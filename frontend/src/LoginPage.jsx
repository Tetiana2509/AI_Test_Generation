import { useState } from "react";
import "./App.css";

function LoginPage({ onLogin }) {
  const [isRegister, setIsRegister] = useState(false);
  const [userName, setUserName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [role, setRole] = useState("Student");

  const handleSubmit = async () => {
    if (!email || !password || (isRegister && !userName)) {
      alert("Заповніть всі поля!");
      return;
    }

    const url = isRegister
      ? "http://localhost:5048/api/users/register"
      : `http://localhost:5048/api/users/login`;

    const payload = isRegister
      ? { userName, email, password, role }
      : { email, password };

    const response = await fetch(url, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
    });

    const data = await response.json();

    if (response.ok) {
      localStorage.setItem("userId", data.id);
      localStorage.setItem("role", data.role);
      console.log("ID користувача:", data.id);
      console.log("Роль:", data.role);
      onLogin(); // запуск переходу на CoursesPage
    } else {
      alert(data.message || "Помилка авторизації/реєстрації");
    }
  };

  return (
    <div className="layout">
      <header className="header-title">AI TEST</header>
      <div className="main-container">
        <main className="content">
          <div className="modal-content" style={{ maxWidth: "400px", margin: "auto" }}>
            <h2>{isRegister ? "Реєстрація" : "Вхід"}</h2>
            {isRegister && (
              <input
                type="text"
                placeholder="Ім’я користувача"
                value={userName}
                onChange={(e) => setUserName(e.target.value)}
              />
            )}
            <input
              type="email"
              placeholder="Email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
            <input
              type="password"
              placeholder="Пароль"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
            {isRegister && (
              <select value={role} onChange={(e) => setRole(e.target.value)}>
                <option value="Student">Студент</option>
                <option value="Teacher">Викладач</option>
              </select>
            )}
            <button className="button" onClick={handleSubmit}>
              {isRegister ? "Зареєструватися" : "Увійти"}
            </button>
            <button className="button" onClick={() => setIsRegister(!isRegister)}>
              {isRegister ? "У мене вже є акаунт" : "Створити акаунт"}
            </button>
          </div>
        </main>
      </div>
      <footer className="footer">© 2025 AI Test Platform</footer>
    </div>
  );
}

export default LoginPage;
