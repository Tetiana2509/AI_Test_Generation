import { useEffect, useState } from "react";
import "./App.css";

function TestEditorPage({ test, onBack }) {
  const [testText, setTestText] = useState("");
  const [questions, setQuestions] = useState([]);
  const [maxScore, setMaxScore] = useState(0);
  const [showNameModal, setShowNameModal] = useState(false);
  console.log("showNameModal:", showNameModal);
  const [tempName, setTempName] = useState("");
  const [testId, setTestId] = useState(null);



  useEffect(() => {
    if (test) {
      setTestId(test.id);
      setTestText(test.text || "");
      setTempName(test.testName || ""); // залишаємо
    if (!test.testName || test.testName.trim() === "") {
    setShowNameModal(true); // ❌ ВИДАЛИ або закоментуй, якщо він у тебе є
    }

  
      if (!test.testName || test.testName.trim() === "") {
        setShowNameModal(true); // ← відкриваємо одразу!
      }
  
      if (Array.isArray(test.questions)) {
        setQuestions(test.questions);
        calculateMaxScore(test.questions);
      }
    }
  }, [test]);
  
  
  
  

  const calculateMaxScore = (questionsList) => {
    let score = 0;
    questionsList.forEach((q) => {
      const options = q.answerOptions || [];
      const maxWeight = options.reduce((acc, a) => acc + (a.weight || 0), 0);
      score += maxWeight;
    });
    setMaxScore(score);
  };

  const handleAddQuestion = () => {
    const newQuestion = {
      questionText: "",
      answerOptions: []
    };
    const updated = [...questions, newQuestion];
    setQuestions(updated);
    calculateMaxScore(updated);
  };

  const handleQuestionTextChange = (index, value) => {
    const updated = [...questions];
    updated[index].questionText = value;
    setQuestions(updated);
  };

  const handleAddAnswer = (qIndex) => {
    const text = prompt("Варіант відповіді:");
    const weight = parseFloat(prompt("Вага (наприклад 1 або 0.4):"));
    if (!text || isNaN(weight)) return;

    const updated = [...questions];
    updated[qIndex].answerOptions = updated[qIndex].answerOptions || [];
    updated[qIndex].answerOptions.push({ text, weight });

    setQuestions(updated);
    calculateMaxScore(updated);
  };

  const handleSave = async () => {
    if (!testText || testText.trim() === "") {
      alert("Текст тесту не може бути порожнім.");
      return;
    }
  
    if (!tempName || tempName.trim() === "") {
      setShowNameModal(true); 
      return;
    }
  
    const fullTest = {
      id: testId,
      testName: tempName,
      text: testText,
      questions: questions
    };
  
    const response = await fetch(`http://localhost:5048/api/fulltests/${testId}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(fullTest),
    });
  
    if (response.ok) {
      alert("Тест збережено успішно!");
    } else {
      alert("Помилка при збереженні тесту");
    }
  };
  
  

  
  const handleModalSave = () => {
    if (!tempName.trim()) {
      alert("Введіть назву тесту");
      return;
    }
    setShowNameModal(false);
    handleSave();
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
          <button className="button" onClick={onBack}>⬅ Назад</button>

          <h2>Текст тесту</h2>
          <textarea
            className="edit-textarea"
            value={testText}
            onChange={(e) => setTestText(e.target.value)}
          />

          <h3>Питання</h3>
          {questions.map((q, idx) => (
            <div key={idx} className="test-card">
              <input
                type="text"
                value={q.questionText}
                placeholder="Текст питання"
                onChange={(e) => handleQuestionTextChange(idx, e.target.value)}
              />
              <ul>
                {(q.answerOptions || []).map((a, i) => (
                  <li key={i}>{a.text} ({a.weight})</li>
                ))}
              </ul>
              <button className="button" onClick={() => handleAddAnswer(idx)}>
                ➕ Додати варіант
              </button>
            </div>
          ))}

          <button className="button" onClick={handleAddQuestion}>➕ Додати питання</button>

          <div className="settings">
            <strong>Максимальний бал: {maxScore}</strong>
          </div>

          <button className="button" onClick={handleSave}>💾 Зберегти тест</button>

            {showNameModal && (
            <div className="modal">
                <div className="modal-content">
                <h3>Введіть назву тесту</h3>
                <input
                    type="text"
                    value={tempName}
                    onChange={(e) => setTempName(e.target.value)}
                />
                <button className="button" onClick={handleModalSave}>Зберегти</button>
                <button className="button" onClick={() => setShowNameModal(false)}>Скасувати</button>
                </div>
            </div>
            )}


        </main>
      </div>
      <footer className="footer">© 2025 AI Test Platform</footer>
    </div>
  );
}

export default TestEditorPage;
