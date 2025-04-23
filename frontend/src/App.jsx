import { useState } from "react";
import GeneratorPage from "./GeneratorPage";
import EditPage from "./EditPage";
import "./App.css";

function App() {
  const [editingFile, setEditingFile] = useState(null);

  return (
    <>
      {editingFile ? (
        <EditPage
          fileName={editingFile.name}
          fileType={editingFile.type}
          onBack={() => setEditingFile(null)}
        />
      ) : (
        <GeneratorPage onEdit={(file) => setEditingFile(file)} />
      )}
    </>
  );
}

export default App;
