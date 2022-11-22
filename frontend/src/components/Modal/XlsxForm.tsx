import React, {Dispatch, FC, SetStateAction} from "react";
import styled from "styled-components";
import {GoogleSSForm} from "./GoogleSSForm";

 interface xlsxFormProps{
    setImportUrl : Dispatch<SetStateAction<string>>
}

export const XlsxForm : FC = () => {
    return(
        <>
        <DropZone type={"file"} placeholder={'Или перетащите файл сюда'}>
        </DropZone>
        </>
    )
}

const DropZone = styled.input`
  margin: 10px;
  display: flex;
  box-sizing: border-box;
  width: 100%;
  height: 150px;
  border-radius: 4px;
  border: 3px dashed;
  padding: 10px 15px;
  font-size: 14px;
`