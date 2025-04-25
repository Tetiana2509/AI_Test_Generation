import { useState, useEffect } from "react";
import CoursesPage from "./CoursesPage";
import TopicsPage from "./TopicsPage";
import GeneratorPage from "./GeneratorPage";
import EditPage from "./EditPage";
import LoginPage from "./LoginPage";
import "./App.css";

function App() {
  const [selectedCourse, setSelectedCourse] = useState(null);
  const [selectedTopic, setSelectedTopic] = useState(null);
  const [editingFile, setEditingFile] = useState(null);
  const [isLoggedIn, setIsLoggedIn] = useState(false);

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

      {editingFile ? (
        <EditPage
          fileName={editingFile.name}
          fileType={editingFile.type}
          onBack={() => setEditingFile(null)}
        />
      ) : selectedTopic ? (
        <GeneratorPage
          topic={selectedTopic}
          onEdit={(file) => setEditingFile(file)}
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
