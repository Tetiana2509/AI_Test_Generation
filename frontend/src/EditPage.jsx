import { useState, useEffect } from "react";
import "./App.css";

function EditPage({ fileName, fileType, onBack }) {
  const [content, setContent] = useState("");
  const [forceReload, setForceReload] = useState(false);
  const [showSuccessMessage, setShowSuccessMessage] = useState(false); 

  useEffect(() => {
    const url = fileType === "tests"
      ? `http://localhost:5048/savedtests/${encodeURIComponent(fileName)}.txt?timestamp=${Date.now()}`
      : `http://localhost:5048/SavedDictionaries/${encodeURIComponent(fileName)}.txt?timestamp=${Date.now()}`;

    fetch(url)
      .then((res) => res.text())
      .then((text) => setContent(text))
      .then(() => setForceReload(false))
      .catch((error) => console.error("Помилка завантаження файлу для редагування:", error));
  }, [fileName, fileType, forceReload]);

  const handleSave = async () => {
    const endpoint = fileType === "tests" ? "test-generation" : "dictionary-generation";

    try {
      const response = await fetch(`http://localhost:5048/api/${endpoint}/save`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ name: fileName, content }),
      });

      if (response.ok) {
        setShowSuccessMessage(true); // Показуємо повідомлення
        setForceReload(true); // Перезавантажуємо файл

        // Сховати повідомлення через 2 секунди
        setTimeout(() => {
          setShowSuccessMessage(false);
        }, 2000);
      } else {
        alert("Помилка при збереженні на сервері");
      }
    } catch (error) {
      console.error("Помилка при збереженні:", error);
      alert("Помилка при збереженні");
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
          <h1 className="title">
            Редагування {fileType === "tests" ? "Тесту" : "Словника"}
          </h1>

          <textarea
            className="edit-textarea"
            value={content}
            onChange={(e) => setContent(e.target.value)}
          ></textarea>

          <div className="actions">
            <button className="button" onClick={onBack}>Назад</button>
            <button className="button" onClick={handleSave}>Зберегти</button>
          </div>

          {/* Сповіщення про успішне збереження */}
          {showSuccessMessage && (
            <div className="success-message">
              ✅ Файл успішно збережено!
            </div>
          )}
        </main>
      </div>
    </div>
  );
}

export default EditPage;
