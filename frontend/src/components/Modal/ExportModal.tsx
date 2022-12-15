import {Modal} from "./Modal";
import {ITablesClient} from "../../clients/TablesClient";
import {FC, useState} from 'react'
import {Lecturer} from "../../Models/Lecturer";
import {GoogleForm} from "./GoogleForm";
import {ColorRing} from "react-loader-spinner";
import styled from "styled-components";

interface exportModalProps{
    closeModal: () => void
    tablesClient: ITablesClient
    lecturers: Lecturer[]
}

export const ExportModal : FC<exportModalProps> = ({tablesClient, lecturers , closeModal}) => {
    const [loading, setLoading] = useState(false)
    const [exportUrl, setExportUrl] = useState('')
    const handleExportSubmit = async () => {
        setLoading(value => !value)
        const regExp = new RegExp("(?<=^([^/]*/){5})([^/]*)")
        const matches = regExp.exec(exportUrl)
        const spreadsheetId = matches![0];
        await tablesClient.exportTableAsync(spreadsheetId, lecturers)
        setLoading(value => !value)
    }
    return(
    <Modal onClose={closeModal} title={'Экспортирование таблицы'} onSubmit={handleExportSubmit}>
        <GoogleForm setUrl={setExportUrl}
        placeholder={'Ссылка на Google Spreadsheet таблицу'}></GoogleForm>
        {loading && <LoadingSpinnerContainer>
            <ColorRing
                visible={true}
                height="80"
                width="80"
                margin-left='100px'
                ariaLabel="blocks-loading"
                wrapperStyle={{}}
                wrapperClass="blocks-wrapper"
                colors={['#e15b64', '#f47e60', '#f8b26a', '#abbd81', '#849b87']}
            />
        </LoadingSpinnerContainer>}
    </Modal>)
}

const LoadingSpinnerContainer = styled.div` 
    align-content: center;
`