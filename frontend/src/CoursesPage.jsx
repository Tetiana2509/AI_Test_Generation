import { useState, useEffect } from "react";
import "./App.css";

function CoursesPage({ onSelectCourse }) {
  const [courses, setCourses] = useState([]);
  const [showModal, setShowModal] = useState(false);
  const [newCourseName, setNewCourseName] = useState("");
  const [joinCode, setJoinCode] = useState("");

  useEffect(() => {
    fetchCourses();
  }, []);

  const fetchCourses = async () => {
    try {
      const userId = localStorage.getItem("userId");
      const role = localStorage.getItem("role");

      const endpoint =
        role === "Teacher"
          ? `http://localhost:5048/api/courses/created/${userId}`
          : `http://localhost:5048/api/courses/joined/${userId}`;

      const response = await fetch(endpoint);
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
  
    const userId = parseInt(localStorage.getItem("userId"));
    
    console.log("userId у localStorage:", userId);
  
    try {
      const response = await fetch("http://localhost:5048/api/courses", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          courseName: newCourseName,
          createdByUserId: userId,
        }),
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
  
  
  

  const handleJoinCourse = async () => {
    const userId = localStorage.getItem("userId");
    if (!joinCode.trim()) {
      alert("Введіть код курсу");
      return;
    }

    try {
      const response = await fetch(
        `http://localhost:5048/api/courses/join?userId=${userId}&courseId=${joinCode}`,
        {
          method: "POST",
        }
      );

      if (response.ok) {
        alert("Успішно приєднано до курсу!");
        setJoinCode("");
        fetchCourses();
      } else {
        const err = await response.json();
        alert(err.message || "Не вдалося приєднатись до курсу");
      }
    } catch (error) {
      console.error("Помилка приєднання:", error);
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

  const handleLeaveCourse = async (courseId) => {
    const userId = parseInt(localStorage.getItem("userId"));
  
    if (!window.confirm("Ви впевнені, що хочете покинути курс?")) return;
  
    try {
      const response = await fetch(`http://localhost:5048/api/courses/leave?userId=${userId}&courseId=${courseId}`, {
        method: "DELETE",
      });
  
      if (response.ok) {
        alert("Ви покинули курс");
        fetchCourses(); 
      } else {
        alert("Помилка при виході з курсу");
      }
    } catch (error) {
      console.error("Помилка виходу з курсу:", error);
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

          {localStorage.getItem("role") === "Teacher" && (
            <button className="button" onClick={() => setShowModal(true)}>
              ➕ Створити курс
            </button>
          )}

          {localStorage.getItem("role") === "Student" && (
            <div style={{ marginTop: "20px" }}>
              <h3>Приєднатися до курсу</h3>
              <input
                type="number"
                placeholder="Введіть код курсу"
                value={joinCode}
                onChange={(e) => setJoinCode(e.target.value)}
              />
              <button className="button" onClick={handleJoinCourse}>
                Приєднатися
              </button>
            </div>
          )}

          <div className="test-list">
            {courses.map((course) => (
              <div key={course.id} className="test-card">
                <div
                  style={{ cursor: "pointer" }}
                  onClick={() => onSelectCourse(course)}
                >
                  <strong>{course.courseName}</strong>
                  <div className="subhead">ID курсу: {course.id}</div>
                </div>
                {localStorage.getItem("role") === "Teacher" && (
                  <button
                    className="icon"
                    onClick={() => handleDeleteCourse(course.id)}
                  >
                    🗑️
                  </button>
                )}
                {localStorage.getItem("role") === "Student" && (
                  <button
                    className="icon"
                    title="Вийти з курсу"
                    onClick={() => handleLeaveCourse(course.id)}
                  >
                    🚪
                  </button>
                )}
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
