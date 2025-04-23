import { useState, useEffect } from "react";
import "./App.css";

function EditPage({ fileName, fileType, onBack }) {
  const [content, setContent] = useState("");
  const [forceReload, setForceReload] = useState(false);
  const [showSuccessMessage, setShowSuccessMessage] = useState(false); // нове для повідомлення

  useEffect(() => {
    const url = fileType === "tests"
      ? `http://localhost:5048/savedtests/${encodeURIComponent(fileName)}.txt?timestamp=${Date.now()}`
      : `http://localhost:5048/SavedDictionaries/${encodeURIComponent(fileName)}.txt?timestamp=${Date.now()}`;

    fetch(url)
      .then((res) => res.text())
      .then((text) => setContent(text))
      .then(() => setForceReload(false))
      .catch((error) => console.error("Ошибка загрузки файла для редактирования:", error));
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
        alert("Ошибка при сохранении на сервере");
      }
    } catch (error) {
      console.error("Ошибка при сохранении:", error);
      alert("Ошибка при сохранении");
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
            Редактирование {fileType === "tests" ? "Теста" : "Словаря"}
          </h1>

          <textarea
            className="edit-textarea"
            value={content}
            onChange={(e) => setContent(e.target.value)}
          ></textarea>

          <div className="actions">
            <button className="button" onClick={onBack}>Назад</button>
            <button className="button" onClick={handleSave}>Сохранить</button>
          </div>

          {/* Уведомление об успешном сохранении */}
          {showSuccessMessage && (
            <div className="success-message">
              ✅ Файл успешно сохранен!
            </div>
          )}
        </main>
      </div>
    </div>
  );
}

export default EditPage;
