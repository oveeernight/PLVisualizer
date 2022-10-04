import React, {FC, useState} from "react";
import styled from "styled-components";
import {Actions} from "./Actions";


export interface appBarProps{
    onImportClick: () => void
    onExportClick: () => void
}

const AppBarContainer = styled.nav`
  color: white;
  height: 80px;
  width: 100%;
  background: black;
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-family: "Segoe UI"
`

const AppNameContainer = styled.div`
    padding-left: 20px;
    color: white;
    font-weight: 500;
    cursor: pointer;
    font-size: xx-large;
`

export const AppBar : FC<appBarProps> = ({onImportClick, onExportClick} : appBarProps) => {
    return(
        <AppBarContainer>
            <AppNameContainer> PLVisualizer </AppNameContainer>
            <Actions onExportClick={onExportClick} onImportClick={onImportClick}  />
        </AppBarContainer>
    )
}

