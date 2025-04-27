import { useState, useEffect } from "react";
import CoursesPage from "./CoursesPage";
import TopicsPage from "./TopicsPage";
import GeneratorPage from "./GeneratorPage";
import EditPage from "./EditPage";
import LoginPage from "./LoginPage";
import TestEditorPage from "./TestEditorPage";
import TestPassingPage from "./TestPassingPage";
import TestHistoryPage from "./TestHistoryPage";
import "./App.css";

function App() {
  const [selectedCourse, setSelectedCourse] = useState(null);
  const [selectedTopic, setSelectedTopic] = useState(null);
  const [editingFile, setEditingFile] = useState(null);
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [editingTestId, setEditingTestId] = useState(null);
  const [editingFullTest, setEditingFullTest] = useState(null);
  const [passingTestId, setPassingTestId] = useState(null);
  const [viewingTestIdForHistory, setViewingTestIdForHistory] = useState(null);




  useEffect(() => {
    const userId = localStorage.getItem("userId");
    setIsLoggedIn(!!userId);
  }, []);

  const handleLogin = () => {
    setIsLoggedIn(true);
  };

  const handleLogout = () => {
    localStorage.clear(); // або removeItem("userId") + removeItem("role")
    setSelectedCourse(null);
    setSelectedTopic(null);
    setEditingFile(null);
    setIsLoggedIn(false);
  };

  const handleCourseSelect = (course) => {
    setSelectedCourse(course);
    setSelectedTopic(null);
    setEditingFile(null);
  };

  const handleTopicSelect = (topic) => {
    setSelectedTopic(topic);
    setEditingFile(null);
  };

  const handleBackToCourses = () => {
    setSelectedCourse(null);
    setSelectedTopic(null);
    setEditingFile(null);
  };

  const handleBackToTopics = () => {
    setSelectedTopic(null);
    setEditingFile(null);
  };

  if (!isLoggedIn) {
    return <LoginPage onLogin={handleLogin} />;
  }

  return (
    <>
      <div style={{ position: "absolute", top: 10, right: 20 }}>
        <button className="button" onClick={handleLogout}>🚪 Вийти</button>
      </div>
  
      {passingTestId ? (
        <TestPassingPage 
          testId={passingTestId} 
          onBack={() => setPassingTestId(null)}
        />
      ) : viewingTestIdForHistory ? (  // <<< НОВА ПЕРЕВІРКА
        <TestHistoryPage
          testId={viewingTestIdForHistory}
          onBack={() => setViewingTestIdForHistory(null)}
        />
      ) : editingFullTest ? (
        <TestEditorPage
          test={editingFullTest}
          onBack={() => setEditingFullTest(null)}
          onViewHistory={(testId) => setViewingTestIdForHistory(testId)} // <<< Додаємо
        />
      ) : editingFile ? (
        <EditPage
          fileName={editingFile.name}
          fileType={editingFile.type}
          onBack={() => setEditingFile(null)}
        />
      ) : selectedTopic ? (
        <GeneratorPage
          topic={selectedTopic}
          onEdit={(file) => setEditingFile(file)}
          onEditTest={(testObjectOrId) => {
            if (typeof testObjectOrId === "object") {
              setEditingFullTest(testObjectOrId);
            } else {
              fetch(`http://localhost:5048/api/fulltests/${testObjectOrId}`)
                .then(res => res.json())
                .then(fullTest => setEditingFullTest(fullTest));
            }
          }}
          onPassTest={(testId) => setPassingTestId(testId)}
          onBack={handleBackToTopics}
        />
      ) : selectedCourse ? (
        <TopicsPage
          course={selectedCourse}
          onSelectTopic={handleTopicSelect}
          onBack={handleBackToCourses}
        />
      ) : (
        <CoursesPage onSelectCourse={handleCourseSelect} />
      )}
    </>
  );
  
}

export default App;
