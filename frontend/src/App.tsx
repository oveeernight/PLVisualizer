import React, {FC, useState} from 'react';
import './App.css';
import {AppBar} from "./components/Appbar/Appbar";
import {Modal} from "./components/Modal/Modal";
import {SelectImport} from "./components/Modal/SelectImport";
import {DragDropRegion} from "./components/DragDropRegion/DragDropRegion";
import {GoogleSSForm} from "./components/Modal/GoogleSSForm";
import {Lecturer} from "./Models/Lecturer"
import {Discipline} from "./Models/Discipline";
import {createTablesClient} from "./clients/TablesClient";
import {ExportModal} from "./components/Modal/ExportModal";
import {ImportModal} from "./components/Modal/ImportModal";


function App() {
    const [importModal, setImportModal] = useState(false)
    const [exportModal, setExportModal] = useState(false)
    const toggleImportModal = () => setImportModal(value => !value)
    const toggleExportModal = () => setExportModal(value => !value)

    const [lecturers, setLecturers] = useState([] as Lecturer[])
    const [columnDisciplines, setColumnDisciplines] = useState([] as Discipline[])

    const tablesClient = createTablesClient()

    return(<div className="Appbar">
            <AppBar onImportClick={toggleImportModal} onExportClick={toggleExportModal}></AppBar>
            { importModal && <ImportModal
                setLecturers={setLecturers}
                tablesClient={tablesClient}
                onClose={toggleImportModal}/>
            }
            { exportModal && <ExportModal
                onClose={toggleExportModal}
                tablesClient={tablesClient}
                lecturers={lecturers} />
            }
            <DragDropRegion  lecturers={lecturers} setLecturers={setLecturers} columnDisciplines={columnDisciplines}
                             setColumnDisciplines={setColumnDisciplines} />
    </div>
    )
}

export default App;
