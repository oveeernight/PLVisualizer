import React, {FC} from "react";
import styled from "styled-components"
import {Lecturer} from "./Lecturer"
import {Droppable} from "react-beautiful-dnd";

interface columnProps {
    title: string
    children: string[]
    id: string
}

const Container = styled.div`
    margin: 8px;
    border: 1px solid lightblue; 
    border-radius: 2px`

const Title = styled.h3`
    padding: 8px;`

const Children = styled.div`
    padding: 8px`

export const Column : FC<columnProps> = ({title, children, id} : columnProps) =>{
    return ( <Container>
            <Title>{title}</Title>
            <Droppable droppableId={id}>
                {(provided) =>(
                    <Children ref={provided.innerRef}>
                        {children.map((child, index) => {
                            return(<Lecturer name={child} index={index}/>)
                        })}
                        {provided.placeholder}
                    </Children>)}
            </Droppable>
        </Container>
    )
}