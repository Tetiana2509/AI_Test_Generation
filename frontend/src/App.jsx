import { useState, useEffect } from "react";
import "./App.css";

function App() {
  const [selectedFile, setSelectedFile] = useState(null);
  const [fileText, setFileText] = useState("");
  const [questionCount, setQuestionCount] = useState("");
  const [difficulty, setDifficulty] = useState("середній");
  const [testResult, setTestResult] = useState("");
  const [files, setFiles] = useState([]);
  const [selectedFileName, setSelectedFileName] = useState("");
  const [savedTests, setSavedTests] = useState([]);
  const [showModal, setShowModal] = useState(false);
  const [testName, setTestName] = useState("");

  useEffect(() => {
    fetch("http://localhost:5048/api/files")
      .then((res) => res.json())
      .then((data) => setFiles(data))
      .catch((error) => console.error("Ошибка загрузки файлов:", error));

    fetch("http://localhost:5048/api/test-generation/saved")
      .then((res) => res.json())
      .then((data) => setSavedTests(data))
      .catch((error) => console.error("Ошибка загрузки сохраненных тестов:", error));
  }, []);

  const handleFileChange = (event) => {
    setSelectedFile(event.target.files[0]);
  };

  const handleUpload = async () => {
    if (!selectedFile) {
      alert("Выберите файл!");
      return;
    }
    const formData = new FormData();
    formData.append("file", selectedFile);

    const response = await fetch("http://localhost:5048/api/files/upload", {
      method: "POST",
      body: formData,
    });

    if (response.ok) {
      alert("Файл загружен!");
      const updatedFiles = await fetch("http://localhost:5048/api/files").then((res) => res.json());
      setFiles(updatedFiles);
    } else {
      alert("Ошибка загрузки файла");
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
      alert("Заполните все поля!");
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

  const handleSaveTest = async () => {
    if (!testName || !testResult) return;

    const response = await fetch("http://localhost:5048/api/test-generation/save", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ name: testName, content: testResult }),
    });

    if (response.ok) {
      alert("Тест сохранён!");
      setShowModal(false);
      setTestName("");
      const updated = await fetch("http://localhost:5048/api/test-generation/saved").then((res) => res.json());
      setSavedTests(updated);
    }
  };

  const handleDownloadSaved = async (testName) => {
    try {
      const response = await fetch(`http://localhost:5048/savedtests/${testName}.txt`);
      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      link.download = `${testName}.txt`;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      URL.revokeObjectURL(url);
    } catch (error) {
      console.error("Ошибка скачивания теста:", error);
      alert("Не удалось скачать файл");
    }
  };

  const handleDownload = () => {
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
          <h1 className="title">Назва теми</h1>

          <div className="test-list">
          {savedTests.map((test) => (
            <div className="test-card" key={test.id}>
              <div>
                <strong>{test.testName}</strong>
                <div className="subhead">Subhead</div>
              </div>
              <div>
                <button className="icon" onClick={() => handleDownloadSaved(test.testName)}>⬇️</button>
                <button className="icon">✏️</button>
              </div>
            </div>
          ))}

          </div>

          <h3>Менеджер файлів</h3>
          <input type="file" onChange={handleFileChange} />
          <button className="button" onClick={handleUpload}>Добавить файл</button>

          <h4>Выберите файл:</h4>
          {files.map((file) => (
            <div key={file.id} className="file-option">
              <input type="radio" name="file" onChange={() => handleSelectFile(file.fileName)} />
              <span>{file.fileName}</span>
            </div>
          ))}

          {fileText && (
            <>
              <button className="button" onClick={handleGenerateTest}>Тест Генерация</button>
              <div className="settings">
                <label>Кількість питань:
                  <input type="number" value={questionCount} onChange={(e) => setQuestionCount(e.target.value)} />
                </label>
                <label>Рівень складності:
                  <select value={difficulty} onChange={(e) => setDifficulty(e.target.value)}>
                    <option value="простий">Простий</option>
                    <option value="середній">Середній</option>
                    <option value="складний">Складний</option>
                  </select>
                </label>
              </div>
            </>
          )}

          {testResult && (
            <div className="test-output">
              <pre>{testResult}</pre>
            </div>
          )}

          {testResult && (
            <div className="actions">
              <button className="button" onClick={() => setShowModal(true)}>Сохранить</button>
              <button className="button" onClick={handleDownload}>Скачать</button>
            </div>
          )}


          {showModal && (
            <div className="modal">
              <div className="modal-content">
                <h3>Введите название теста</h3>
                <input
                  type="text"
                  value={testName}
                  onChange={(e) => setTestName(e.target.value)}
                />
                <button className="button" onClick={handleSaveTest}>Сохранить</button>
                <button className="button" onClick={() => setShowModal(false)}>Отмена</button>
              </div>
            </div>
          )}
        </main>
      </div>
    </div>
  );
}

export default App;
