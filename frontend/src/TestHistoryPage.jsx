import { useEffect, useState } from "react";

function TestHistoryPage({ testId, onBack }) {
  const [submissions, setSubmissions] = useState([]);

  useEffect(() => {
    fetchSubmissions();
  }, []);

  const fetchSubmissions = async () => {
    const response = await fetch(`http://localhost:5048/api/fulltests/submissions/${testId}`);
    const data = await response.json();
    setSubmissions(data);
  };

  const handleDeleteSubmission = async (submissionId) => {
    if (!window.confirm("Ви впевнені, що хочете видалити це проходження?")) return;

    const response = await fetch(`http://localhost:5048/api/fulltests/submission/${submissionId}`, {
      method: "DELETE",
    });

    if (response.ok) {
      alert("Проходження видалено!");
      fetchSubmissions();
    } else {
      alert("Помилка при видаленні!");
    }
  };

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

          <h1 className="title">Історія проходжень тесту</h1>

          <div className="test-list">
            {submissions.length === 0 && <p>Проходжень ще немає.</p>}
            {submissions.map(sub => (
              <div key={sub.id} className="test-card">
                <div>
                  <strong>{sub.name}</strong> ({sub.email})
                  <br />
                  Бал: {sub.score}
                  <br />
                  Дата: {new Date(sub.submittedAt).toLocaleString()}
                </div>
                <div>
                  <button className="icon" onClick={() => handleDeleteSubmission(sub.id)}>🗑️</button>
                </div>
              </div>
            ))}
          </div>
        </main>
      </div>

      <footer className="footer">© 2025 AI Test Platform</footer>
    </div>
  );
}

export default TestHistoryPage;
