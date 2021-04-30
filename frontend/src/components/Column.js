import PropTypes from 'prop-types';
import React, { useContext, useEffect, useState } from 'react';
import Card from 'react-bootstrap/Card';
import Col from 'react-bootstrap/Col';
import { DataContext } from '../contextProvider/DataContextProvider';
import { deleteColumn } from '../util/columnsApi';
import ColumnModal from './modals/ColumnModal';
import DeleteModal from './modals/DeleteModal';
import Placeholder from './Placeholder';
import Todo from './Todo';

export default function Column(props) {
  const [state, dispatch] = useContext(DataContext);
  const [thisColumn, setThisColumn] = useState({});
  const [showColumnModal, setShowColumnModal] = useState(false);
  const [showDeleteModal, setShowDeleteModal] = useState(false);

  const buildTodoComponent = (todo) => <Todo key={todo.id} id={todo.id} />;

  const handleShowColumnModal = () => setShowColumnModal(true);
  const handleCloseColumnModal = () => setShowColumnModal(false);

  const handleShowDeleteModal = () => setShowDeleteModal(true);
  const handleCloseDeleteModal = () => setShowDeleteModal(false);

  useEffect(() => {
    const todoData = state.todos
      ? state.todos.filter((todo) => todo.columnId === props.id)
      : [];
    const columnTitle = state.columns.filter((c) => c.id === props.id)[0].title;
    const thisColumn = {
      id: props.id,
      title: columnTitle,
      todos: todoData.map((todo) => buildTodoComponent(todo)),
    };
    setThisColumn(thisColumn);
  }, [state]);

  const deleteThisColumn = async () => {
    handleCloseDeleteModal();
    await deleteColumn(thisColumn.id);
    const newColumns = state.columns.filter((col) => col.id !== thisColumn.id);
    dispatch({
      type: 'UPDATE_COLUMNS',
      payload: {
        columns: newColumns,
      },
    });
  };

  const columnModal = (
    <ColumnModal
      show={showColumnModal}
      hide={handleCloseColumnModal}
      id={thisColumn.id}
    />
  );
  const deleteModal = (
    <DeleteModal
      show={showDeleteModal}
      hide={handleCloseDeleteModal}
      delete={deleteThisColumn}
      itemType='column'
      title={thisColumn.title ? thisColumn.title : ''}
    />
  );

  return (
    <>
      <Col lg='3' md='4' sm='6' xs='12'>
        <Card>
          <Card.Title className='d-flex justify-content-between text-left mb-1 mt-2 px-3'>
            <div className='align-self-center'>
              {thisColumn.title
                ? thisColumn.title.length > 15
                  ? thisColumn.title.slice(0, 15)
                  : thisColumn.title
                : ''}
            </div>
            <div>
              <a
                className='mx-1 icon edit-icon'
                onClick={handleShowColumnModal}>
                <i className='far fa-edit'></i>
              </a>
              <a
                className='mx-1 icon delete-icon'
                onClick={handleShowDeleteModal}>
                <i className='far fa-trash-alt'></i>
              </a>
            </div>
          </Card.Title>
          <Card.Body className='mx-2 mb-2 mt-1 p-1'>
            {thisColumn.todos}
            <Placeholder columnId={thisColumn.id} type='todo' />
          </Card.Body>
        </Card>
      </Col>
      {columnModal}
      {deleteModal}
    </>
  );
}

Column.propTypes = {
  id: PropTypes.number.isRequired,
};
