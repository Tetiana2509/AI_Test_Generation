import { useState } from "react";
import CoursesPage from "./CoursesPage";
import TopicsPage from "./TopicsPage";
import GeneratorPage from "./GeneratorPage";
import EditPage from "./EditPage";
import "./App.css";

function App() {
  const [selectedCourse, setSelectedCourse] = useState(null);
  const [selectedTopic, setSelectedTopic] = useState(null);
  const [editingFile, setEditingFile] = useState(null);

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

  return (
    <>
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
