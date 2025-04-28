import { useState, useEffect } from "react";

function TestPassingPage({ testId, onBack }) {
  const [test, setTest] = useState(null);
  const [answers, setAnswers] = useState({});
  const [result, setResult] = useState(null);

  useEffect(() => {
    async function fetchTest() {
      const response = await fetch(`http://localhost:5048/api/fulltests/student-view/${testId}`);
      const data = await response.json();
      setTest(data);
    }
    fetchTest();
  }, [testId]);

  const handleAnswerChange = (questionId, answerId) => {
    setAnswers((prev) => {
      const prevAnswers = prev[questionId] || [];
      if (prevAnswers.includes(answerId)) {
        return { ...prev, [questionId]: prevAnswers.filter(id => id !== answerId) };
      } else {
        return { ...prev, [questionId]: [...prevAnswers, answerId] };
      }
    });
  };

  const handleSubmit = async () => {
    const userId = parseInt(localStorage.getItem("userId"));
  
    let totalScore = 0;
  
    // Нараховуємо бали відповідно до ваг
    test.questions.forEach(q => {
      const selectedOptions = answers[q.id] || [];
  
      selectedOptions.forEach(selectedId => {
        const selectedAnswer = q.answerOptions.find(a => a.id === selectedId);
        if (selectedAnswer) {
          totalScore += selectedAnswer.weight; // додаємо вагу обраної відповіді
        }
      });
    });
  
    // Максимальний бал - сума всіх ваг правильних відповідей
    const maxScore = test.questions.reduce((sum, q) => {
      return sum + q.answerOptions
        .filter(opt => opt.weight > 0)
        .reduce((subSum, opt) => subSum + opt.weight, 0);
    }, 0);
  
    const percentage = ((totalScore / maxScore) * 100).toFixed(2);
  
    const payload = {
      userId,
      fullTestId: testId,
      submittedAt: new Date().toISOString(),
      answerSubmissions: Object.entries(answers).flatMap(([questionId, answerIds]) =>
        answerIds.map(answerId => ({
          questionId: parseInt(questionId),
          answerOptionId: answerId
        }))
      )
    };
  
    const response = await fetch("http://localhost:5048/api/fulltests/submit", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
    });
  
    if (response.ok) {
      setResult({ totalScore, maxScore, percentage });
    } else {
      alert("Помилка при проходженні тесту");
    }
  };
  

  if (!test) return <div>Завантаження тесту...</div>;

  if (result) {
    return (
      <div className="layout">
        <header className="header">AI TEST</header>
        <div className="main-container">
          <aside className="sidebar">
            <div className="menu-item">
              <span>📚</span>
              <div><div className="menu-title">Курси</div></div>
            </div>
          </aside>

          <main className="content">
            <h1 className="title">Результат тесту</h1>
            <div className="test-card">
              <h2>Ваш бал: {result.totalScore} із {result.maxScore}</h2>
              <h3>Ваша оцінка: {result.percentage}%</h3>
            </div>
            <button className="button" onClick={onBack}>⬅ Назад</button>
          </main>
        </div>

        <footer className="footer">© 2025 AI Test Platform</footer>
      </div>
    );
  }

  return (
    <div className="layout">
      <header className="header">AI TEST</header>
      <div className="main-container">
        <aside className="sidebar">
          <div className="menu-item">
            <span>📚</span>
            <div><div className="menu-title">Курси</div></div>
          </div>
        </aside>

        <main className="content">
          <button className="button" onClick={onBack}>⬅ Назад</button>

          {/* 🟣 Исправили отображение текста теста */}
          <div className="test-text-output">
            <pre>{test.text}</pre>
          </div>

          {test.questions.map((q) => (
            <div key={q.id} className="test-card">
              <h3>{q.questionText}</h3>
              {q.answerOptions.map((a) => (
                <div key={a.id}>
                  <label>
                    <input
                      type="checkbox"
                      name={`question-${q.id}`}
                      value={a.id}
                      checked={answers[q.id]?.includes(a.id) || false}
                      onChange={() => handleAnswerChange(q.id, a.id)}
                    />
                    {a.text}
                  </label>
                </div>
              ))}
            </div>
          ))}

          <button className="button" onClick={handleSubmit}>✅ Завершити тест</button>
        </main>
      </div>

      <footer className="footer">© 2025 AI Test Platform</footer>
    </div>
  );
}

export default TestPassingPage;
