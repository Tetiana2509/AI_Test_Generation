import { useState, useEffect } from "react";
import "./App.css";

function GeneratorPage({ topic, onEdit, onBack }) {
  const [selectedFile, setSelectedFile] = useState(null);
  const [fileText, setFileText] = useState("");
  const [questionCount, setQuestionCount] = useState("");
  const [difficulty, setDifficulty] = useState("середній");
  const [testResult, setTestResult] = useState("");
  const [dictionaryResult, setDictionaryResult] = useState("");
  const [files, setFiles] = useState([]);
  const [selectedFileName, setSelectedFileName] = useState("");
  const [savedTests, setSavedTests] = useState([]);
  const [savedDictionaries, setSavedDictionaries] = useState([]);
  const [showModal, setShowModal] = useState(false);
  const [saveName, setSaveName] = useState("");
  const [isSavingTest, setIsSavingTest] = useState(true);

  useEffect(() => {
    if (topic) {
      fetchSavedFiles();
    }
  }, [topic]);

  const fetchSavedFiles = async () => {
    try {
      const filesData = await fetch(`http://localhost:5048/api/files/by-topic/${topic.id}`).then(res => res.json());
      setFiles(filesData);
      const testsData = await fetch(`http://localhost:5048/api/test-generation/saved/${topic.id}`).then(res => res.json());
      setSavedTests(testsData);
      const dictionariesData = await fetch(`http://localhost:5048/api/dictionary-generation/saved/${topic.id}`).then(res => res.json());
      setSavedDictionaries(dictionariesData);
    } catch (error) {
      console.error("Помилка завантаження даних:", error);
    }
  };

  const handleFileChange = (event) => {
    setSelectedFile(event.target.files[0]);
  };

  const handleUpload = async () => {
    if (!selectedFile) {
      alert("Оберіть файл!");
      return;
    }
    const formData = new FormData();
    formData.append("file", selectedFile);
    formData.append("topicId", topic.id); // важливо

    const response = await fetch("http://localhost:5048/api/files/upload", {
      method: "POST",
      body: formData,
    });

    if (response.ok) {
      alert("Файл завантажено!");
      fetchSavedFiles();
    } else {
      alert("Помилка завантаження файлу");
    }
  };

  const handleSelectFile = async (fileName) => {
    setSelectedFileName(fileName);
    const response = await fetch(`http://localhost:5048/api/files/extract-text/${fileName}`);
    const data = await response.json();
    setFileText(data.text);
  };

  const handleGenerateTest = async () => {
    if (!fileText || !questionCount) {
      alert("Заповніть усі поля!");
      return;
    }

    const response = await fetch("http://localhost:5048/api/test-generation/generate", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        text: fileText,
        questionCount: parseInt(questionCount),
        difficulty,
      }),
    });

    const result = await response.json();
    setTestResult(result.test);
  };

  const handleGenerateDictionary = async () => {
    if (!fileText) {
      alert("Оберіть файл!");
      return;
    }

    const response = await fetch("http://localhost:5048/api/dictionary-generation/generate", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ text: fileText }),
    });

    const result = await response.json();
    setDictionaryResult(result.dictionary);
  };

  const handleSave = async () => {
    const endpoint = isSavingTest ? "test-generation" : "dictionary-generation";
    const contentToSave = isSavingTest ? testResult : dictionaryResult;

    const response = await fetch(`http://localhost:5048/api/${endpoint}/save`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        name: saveName,
        content: contentToSave,
        topicId: topic.id,
      }),
    });

    if (response.ok) {
      alert("Успішно збережено!");
      setShowModal(false);
      setSaveName("");
      fetchSavedFiles();
    } else {
      alert("Помилка при збереженні!");
    }
  };

  const handleDownloadTest = () => {
    const blob = new Blob([testResult], { type: "text/plain;charset=utf-8" });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = "generated_test.txt";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
  };

  const handleDownloadDictionary = () => {
    const blob = new Blob([dictionaryResult], { type: "text/plain;charset=utf-8" });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = "generated_dictionary.txt";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
  };

  const handleDownloadSaved = async (name, type) => {
    const endpoint = type === "tests" ? "savedtests" : "SavedDictionaries";
    const url = `http://localhost:5048/${endpoint}/${encodeURIComponent(name)}.txt`;

    try {
      const response = await fetch(url);
      if (!response.ok) {
        throw new Error("Помилка завантаження файлу");
      }

      const blob = await response.blob();
      const downloadUrl = URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = downloadUrl;
      link.download = `${name}.txt`;
      document.body.appendChild(link);
      link.click();
      link.remove();
      URL.revokeObjectURL(downloadUrl);
    } catch (error) {
      console.error("Помилка при завантаженні файлу:", error);
      alert("Не вдалося завантажити файл");
    }
  };

  const handleDeleteSaved = async (name, type) => {
    const confirmDelete = window.confirm(`Ви впевнені, що хочете видалити ${name}?`);
    if (!confirmDelete) return;

    const endpoint = type === "tests" ? "test-generation" : "dictionary-generation";

    try {
      const response = await fetch(`http://localhost:5048/api/${endpoint}/delete/${name}`, {
        method: "DELETE",
      });

      if (response.ok) {
        alert(`${type === "tests" ? "Тест" : "Словник"} видалено!`);
        fetchSavedFiles();
      } else {
        alert("Помилка видалення");
      }
    } catch (error) {
      console.error("Помилка при видаленні:", error);
      alert("Помилка видалення");
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
        <button
          className="button"
          style={{ marginBottom: "20px" }}
          onClick={onBack}
        >
          ⬅ Назад до теми
        </button>

          <h1 className="title">{topic.topicName}</h1>

          <h2>Збережені тести</h2>
          <div className="test-list">
            {savedTests.map((test) => (
              <div className="test-card" key={test.id}>
                <div>
                  <strong>{test.testName}</strong>
                  <div className="subhead">Subhead</div>
                </div>
                <div>
                  <button className="icon" onClick={() => handleDownloadSaved(test.testName, "tests")}>⬇️</button>
                  <button className="icon" onClick={() => onEdit({ name: test.testName, type: "tests" })}>✏️</button>
                  <button className="icon" onClick={() => handleDeleteSaved(test.testName, "tests")}>🗑️</button>
                </div>
              </div>
            ))}
          </div>

          <h2>Збережені словники</h2>
          <div className="test-list">
            {savedDictionaries.map((dictionary) => (
              <div className="test-card" key={dictionary.id}>
                <div>
                  <strong>{dictionary.dictionaryName}</strong>
                  <div className="subhead">Subhead</div>
                </div>
                <div>
                  <button className="icon" onClick={() => handleDownloadSaved(dictionary.dictionaryName, "dictionaries")}>⬇️</button>
                  <button className="icon" onClick={() => onEdit({ name: dictionary.dictionaryName, type: "dictionaries" })}>✏️</button>
                  <button className="icon" onClick={() => handleDeleteSaved(dictionary.dictionaryName, "dictionaries")}>🗑️</button>
                </div>
              </div>
            ))}
          </div>

          <h1 className="title">Менеджер файлів</h1>

          {localStorage.getItem("role") === "Teacher" && (
            <>
              <input type="file" onChange={handleFileChange} />
              <button className="button" onClick={handleUpload}>Додати файл</button>
            </>
          )}


          <h4>Оберіть файл:</h4>
          {files.map((file) => (
            <div key={file.id} className="file-option">
              <input type="radio" name="file" onChange={() => handleSelectFile(file.fileName)} />
              <span>{file.fileName}</span>
            </div>
          ))}

          {/* Генерація тесту */}
          <section className="generator-section">
            <h2>Генерація тесту</h2>
            <button className="button" onClick={handleGenerateTest} disabled={!fileText}>Згенерувати тест</button>
            <div className="settings">
              <label>Кількість питань:
                <input type="number" value={questionCount} onChange={(e) => setQuestionCount(e.target.value)} disabled={!fileText} />
              </label>
              <label>Рівень складності:
                <select value={difficulty} onChange={(e) => setDifficulty(e.target.value)} disabled={!fileText}>
                  <option value="простий">Простий</option>
                  <option value="середній">Середній</option>
                  <option value="складний">Складний</option>
                </select>
              </label>
            </div>

            {testResult && (
              <div className="test-output">
                <pre>{testResult}</pre>
              </div>
            )}

            {testResult && (
              <div className="actions">
                <button className="button" onClick={() => { setIsSavingTest(true); setShowModal(true); }}>Зберегти тест</button>
                <button className="button" onClick={handleDownloadTest}>Завантажити тест</button>
              </div>
            )}
          </section>

          {/* Генерація словника */}
          <section className="generator-section">
            <h2>Генерація словника</h2>
            <button className="button" onClick={handleGenerateDictionary} disabled={!fileText}>Згенерувати словник</button>

            {dictionaryResult && (
              <div className="test-output">
                <pre>{dictionaryResult}</pre>
              </div>
            )}

            {dictionaryResult && (
              <div className="actions">
                <button className="button" onClick={() => { setIsSavingTest(false); setShowModal(true); }}>Зберегти словник</button>
                <button className="button" onClick={handleDownloadDictionary}>Завантажити словник</button>
              </div>
            )}
          </section>

          {/* Модальне вікно збереження */}
          {showModal && (
            <div className="modal">
              <div className="modal-content">
                <h3>Введіть назву</h3>
                <input type="text" value={saveName} onChange={(e) => setSaveName(e.target.value)} />
                <button className="button" onClick={handleSave}>Зберегти</button>
                <button className="button" onClick={() => setShowModal(false)}>Скасувати</button>
              </div>
            </div>
          )}
        </main>
      </div>
      <footer className="footer">© 2025 AI Test Platform. All rights reserved.</footer>
    </div>
  );
}

export default GeneratorPage;
