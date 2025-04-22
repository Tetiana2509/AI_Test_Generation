import { useState, useEffect } from "react";
import "./App.css";

function GeneratorPage({ onEdit }) {
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

  const [isGeneratingTest, setIsGeneratingTest] = useState(true); 
  const [savedDictionaries, setSavedDictionaries] = useState([]); 

  useEffect(() => {
    fetch("http://localhost:5048/api/files")
      .then((res) => res.json())
      .then((data) => setFiles(data))
      .catch((error) => console.error("Ошибка загрузки файлов:", error));

    fetch("http://localhost:5048/api/test-generation/saved")
      .then((res) => res.json())
      .then((data) => setSavedTests(data))
      .catch((error) => console.error("Ошибка загрузки сохраненных тестов:", error));

    fetch("http://localhost:5048/api/dictionary-generation/saved")
      .then((res) => res.json())
      .then((data) => setSavedDictionaries(data))
      .catch((error) => console.error("Ошибка загрузки сохраненных словарей:", error));
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

  const handleGenerateDictionary = async () => {
    if (!fileText) {
      alert("Выберите файл!");
      return;
    }

    const response = await fetch("http://localhost:5048/api/dictionary-generation/generate", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ text: fileText }),
    });

    const result = await response.json();
    setTestResult(result.dictionary);
  };

  const handleSaveTest = async () => {
    if (!testName || !testResult) return;

    const endpoint = isGeneratingTest ? "test-generation" : "dictionary-generation";

    const response = await fetch(`http://localhost:5048/api/${endpoint}/save`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ name: testName, content: testResult }),
    });

    if (response.ok) {
      alert(isGeneratingTest ? "Тест сохранён!" : "Словарь сохранён!");
      setShowModal(false);
      setTestName("");

      if (isGeneratingTest) {
        const updated = await fetch("http://localhost:5048/api/test-generation/saved").then((res) => res.json());
        setSavedTests(updated);
      } else {
        const updated = await fetch("http://localhost:5048/api/dictionary-generation/saved").then((res) => res.json());
        setSavedDictionaries(updated);
      }
    }
  };

  const handleDownload = () => {
    const blob = new Blob([testResult], { type: "text/plain;charset=utf-8" });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = isGeneratingTest ? "generated_test.txt" : "generated_dictionary.txt";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
  };

  const handleDeleteSaved = async (name, type) => {
    const confirmDelete = window.confirm(`Вы уверены, что хотите удалить ${name}?`);
    if (!confirmDelete) return;
  
    const endpoint = type === "tests" ? "test-generation" : "dictionary-generation";
  
    try {
      const response = await fetch(`http://localhost:5048/api/${endpoint}/delete/${name}`, {
        method: "DELETE",
      });
  
      if (response.ok) {
        alert(`${type === "tests" ? "Тест" : "Словарь"} удалён!`);
  
        // Обновляем список после удаления
        if (type === "tests") {
          const updated = await fetch("http://localhost:5048/api/test-generation/saved").then(res => res.json());
          setSavedTests(updated);
        } else {
          const updated = await fetch("http://localhost:5048/api/dictionary-generation/saved").then(res => res.json());
          setSavedDictionaries(updated);
        }
      } else {
        alert("Ошибка удаления");
      }
    } catch (error) {
      console.error("Ошибка при удалении:", error);
      alert("Ошибка удаления");
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
          <h1 className="title">Назва теми</h1>

          <h2>Сохраненные тесты</h2>
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

        <h2>Сохраненные словари</h2>
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
              <div style={{ display: 'flex', gap: '10px', marginTop: '20px' }}>
                <button className="button" onClick={() => { setIsGeneratingTest(true); handleGenerateTest(); }}>Тест Генерация</button>
                <button className="button" onClick={() => { setIsGeneratingTest(false); handleGenerateDictionary(); }}>Генерация Словаря</button>
              </div>

              {isGeneratingTest && (
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
              )}
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
                <h3>Введите название {isGeneratingTest ? "теста" : "словаря"}</h3>
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
      <footer className="footer">
        © 2025 AI Test Platform. All rights reserved.
    </footer>

    </div>
  );
}

export default GeneratorPage;
