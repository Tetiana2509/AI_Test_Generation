import { useState, useEffect } from "react";
import "./App.css";

function CoursesPage({ onSelectCourse }) {
  const [courses, setCourses] = useState([]);
  const [showModal, setShowModal] = useState(false);
  const [newCourseName, setNewCourseName] = useState("");

  useEffect(() => {
    fetchCourses();
  }, []);

  const fetchCourses = async () => {
    try {
      const response = await fetch("http://localhost:5048/api/courses");
      const data = await response.json();
      setCourses(data);
    } catch (error) {
      console.error("Помилка завантаження курсів:", error);
    }
  };

  const handleCreateCourse = async () => {
    if (!newCourseName.trim()) {
      alert("Введіть назву курсу!");
      return;
    }

    try {
      const response = await fetch("http://localhost:5048/api/courses", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ courseName: newCourseName }),
      });

      if (response.ok) {
        setShowModal(false);
        setNewCourseName("");
        fetchCourses();
      } else {
        alert("Помилка при створенні курсу");
      }
    } catch (error) {
      console.error("Помилка створення курсу:", error);
    }
  };

  const handleDeleteCourse = async (id) => {
    if (!window.confirm("Ви впевнені, що хочете видалити курс?")) return;

    try {
      const response = await fetch(`http://localhost:5048/api/courses/${id}`, {
        method: "DELETE",
      });

      if (response.ok) {
        fetchCourses();
      } else {
        alert("Помилка при видаленні курсу");
      }
    } catch (error) {
      console.error("Помилка видалення курсу:", error);
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
          <h1 className="title">Курси</h1>

          <button className="button" onClick={() => setShowModal(true)}>
            ➕ Створити курс
          </button>

          <div className="test-list">
            {courses.map((course) => (
              <div key={course.id} className="test-card">
                <div
                  style={{ cursor: "pointer" }}
                  onClick={() => onSelectCourse(course)}
                >
                  <strong>{course.courseName}</strong>
                  <div className="subhead">Subhead</div>
                </div>
                <button
                  className="icon"
                  onClick={() => handleDeleteCourse(course.id)}
                >
                  🗑️
                </button>
              </div>
            ))}
          </div>

          {showModal && (
            <div className="modal">
              <div className="modal-content">
                <h3>Назва курсу</h3>
                <input
                  type="text"
                  value={newCourseName}
                  onChange={(e) => setNewCourseName(e.target.value)}
                />
                <button className="button" onClick={handleCreateCourse}>
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

export default CoursesPage;
