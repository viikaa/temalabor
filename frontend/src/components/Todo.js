import PropTypes from 'prop-types';
import React, { useContext, useEffect, useState } from 'react';
import Card from 'react-bootstrap/Card';
import { DataContext } from '../contextProvider/DataContextProvider';
import { deleteTodo } from '../util/todosApi';
import DeleteModal from './modals/DeleteModal';
import TodoModal from './modals/TodoModal';

export default function Todo(props) {
  const [state, dispatch] = useContext(DataContext);
  const [thisTodo, setThisTodo] = useState({});

  const [showTodoModal, setShowTodoModal] = useState(false);
  const [showDeleteModal, setShowDeleteModal] = useState(false);

  useEffect(() => {
    setThisTodo(state.todos.filter((t) => t.id === props.id)[0]);
  }, [state]);

  const handleCloseTodoModal = () => setShowTodoModal(false);
  const handleShowTodoModal = () => setShowTodoModal(true);

  const handleCloseDeleteModal = () => setShowDeleteModal(false);
  const handleShowDeleteModal = () => setShowDeleteModal(true);

  const deleteThisTodo = async () => {
    await deleteTodo(thisTodo.id);
    const newTodos = state.todos.filter((t) => t.id !== thisTodo.id);
    dispatch({
      type: 'UPDATE_TODOS',
      payload: {
        todos: newTodos,
      },
    });
  };

  const todoModal = (
    <TodoModal
      show={showTodoModal}
      hide={handleCloseTodoModal}
      id={thisTodo.id}
      columnId={thisTodo.columnId ? thisTodo.columnId : -1}
    />
  );
  const deleteModal = (
    <DeleteModal
      show={showDeleteModal}
      hide={handleCloseDeleteModal}
      delete={deleteThisTodo}
      itemType='todo'
      title={thisTodo.title ? thisTodo.title : ''}
    />
  );

  return (
    <>
      <Card
        text='light'
        bg='secondary'
        className='todo py-3 px-1 mx-0 my-2 text-left d-flex flex-row justify-content-between'>
        <div className='align-self-center'>
          {thisTodo.title
            ? thisTodo.title.length > 15
              ? thisTodo.title.slice(0, 15)
              : thisTodo.title
            : ''}
        </div>
        <div>
          <a className='mx-1 icon edit-icon' onClick={handleShowTodoModal}>
            <i className='far fa-edit'></i>
          </a>
          <a className='mx-1 icon delete-icon' onClick={handleShowDeleteModal}>
            <i className='far fa-trash-alt'></i>
          </a>
        </div>
      </Card>

      {todoModal}
      {deleteModal}
    </>
  );
}

Todo.propTypes = {
  id: PropTypes.number.isRequired,
};
