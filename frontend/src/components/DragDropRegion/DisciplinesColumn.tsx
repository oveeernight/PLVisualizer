import React, {FC, useState} from "react";
import styled from "styled-components"
import {Droppable} from "react-beautiful-dnd";
import {Discipline} from "./Discipline";
import {ITableRow} from "./ITableRow";

export interface columnProps {
    handleResetClick : () => void
    disciplineIds: string[]
}

interface disciplinesListProps {
    readonly isDraggingOver: boolean
}

export const DisciplinesColumn : FC<columnProps> = ({disciplineIds, handleResetClick}) =>{
    return ( <Container>
            <Title>Дисциплины</Title>
            <Droppable droppableId={'column'}>
                {(provided, snapshot) =>(
                    <DisciplinesList ref={provided.innerRef} isDraggingOver = {snapshot.isDraggingOver}>
                        {disciplineIds.map((id, index) => {
                            return(<Discipline key={id} name={id} index={index}/>)
                        })}
                        {provided.placeholder}
                    </DisciplinesList>)}
            </Droppable>
            <ButtonContainer onClick={handleResetClick}> Сбросить </ButtonContainer>
        </Container>
    )
}

const Container = styled.div`
  display: flex;
  flex-direction: column;
    width: 15%;
    margin: 8px;
    border: 1px solid lightblue; 
    border-radius: 2px`

const Title = styled.h3`
    padding: 8px;`

const DisciplinesList = styled.div<disciplinesListProps>`
  min-height: 50px;
    padding: 8px;
  background-color: ${(props) => props.isDraggingOver ? 'skyblue' : 'white'}`

const ButtonContainer = styled.button`
  backface-visibility: hidden;
  background-color: white;
  border: 2px solid skyblue;
  border-radius: 12px;
  box-sizing: border-box;
  color: black;
  cursor: pointer;
  display: inline-block;
  font-family: "Segoe UI";
  font-size: 16px;
  font-weight: 600;
  letter-spacing: normal;
  line-height: 1.5;
  overflow: visible ;
  padding: 13px 20px;
  position: relative;
  text-align: center;
  text-decoration: none;
  transform: translate3d(0, 0, 0);
  transition: all .3s;
`