// src/pages/TopicsPage.jsx
import { useState, useEffect } from "react";
import "./App.css";

function TopicsPage({ course, onBack, onSelectTopic }) {
  const [topics, setTopics] = useState([]);
  const [showModal, setShowModal] = useState(false);
  const [newTopicName, setNewTopicName] = useState("");

  

  useEffect(() => {
    fetchTopics();
  }, []);

  const fetchTopics = async () => {
    try {
      const response = await fetch(`http://localhost:5048/api/topics/by-course/${course.id}`);
      const data = await response.json();
      setTopics(data);
    } catch (error) {
      console.error("Помилка завантаження тем:", error);
    }
  };

  const handleCreateTopic = async () => {
    if (!newTopicName.trim()) {
      alert("Введіть назву теми!");
      return;
    }

    try {
      const response = await fetch("http://localhost:5048/api/topics", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ topicName: newTopicName, courseId: course.id }),
      });

      if (response.ok) {
        setShowModal(false);
        setNewTopicName("");
        fetchTopics();
      } else {
        alert("Помилка при створенні теми");
      }
    } catch (error) {
      console.error("Помилка створення теми:", error);
    }
  };

  const handleDeleteTopic = async (id) => {
    if (!window.confirm("Ви впевнені, що хочете видалити тему?")) return;

    try {
      const response = await fetch(`http://localhost:5048/api/topics/${id}`, {
        method: "DELETE",
      });

      if (response.ok) {
        fetchTopics();
      } else {
        alert("Помилка при видаленні теми");
      }
    } catch (error) {
      console.error("Помилка видалення теми:", error);
    }
  };

  return (
    <div className="layout">
      <header className="header">AI TEST</header>
      <div className="main-container">
        <aside className="sidebar">
          <div className="menu-item">
            <span>📚</span>
            <div>
              <div className="menu-title">Курси</div>
            </div>
          </div>
        </aside>

        <main className="content">
          <h1 className="title">{course.courseName}</h1>

          <button className="button" onClick={onBack}>
            ← Назад до курсів
          </button>

          {localStorage.getItem("role") === "Teacher" && (
            <button className="button" onClick={() => setShowModal(true)}>
              ➕ Створити тему
            </button>
          )}


          <div className="test-list">
            {topics.map((topic) => (
              <div key={topic.id} className="test-card">
                <div
                  style={{ cursor: "pointer" }}
                  onClick={() => onSelectTopic(topic)}
                >
                  <strong>{topic.topicName}</strong>
                </div>
                {localStorage.getItem("role") === "Teacher" && (
                  <button
                    className="icon"
                    onClick={() => handleDeleteTopic(topic.id)}
                  >
                    🗑️
                  </button>
                )}

              </div>
            ))}
          </div>

          {showModal && (
            <div className="modal">
              <div className="modal-content">
                <h3>Назва теми</h3>
                <input
                  type="text"
                  value={newTopicName}
                  onChange={(e) => setNewTopicName(e.target.value)}
                />
                <button className="button" onClick={handleCreateTopic}>
                  Створити
                </button>
                <button className="button" onClick={() => setShowModal(false)}>
                  Скасувати
                </button>
              </div>
            </div>
          )}
        </main>
      </div>
      <footer className="footer">© 2025 AI Test Platform</footer>
    </div>
  );
}

export default TopicsPage;
