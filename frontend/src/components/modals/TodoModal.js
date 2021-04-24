import PropTypes from 'prop-types';
import React, { useContext, useEffect, useRef, useState } from 'react';
import Alert from 'react-bootstrap/Alert';
import Button from 'react-bootstrap/Button';
import Col from 'react-bootstrap/Col';
import Form from 'react-bootstrap/Form';
import Modal from 'react-bootstrap/Modal';
import Row from 'react-bootstrap/Row';
import { DataContext } from '../../contextProvider/DataContextProvider';
import { addTodo, fetchSingleTodo, updateTodo } from '../../util/todosApi';

export default function TodoModal(props) {
  const [state, dispatch] = useContext(DataContext);
  const [validated, setValidated] = useState(false);
  const [showAlert, setShowAlert] = useState(false);
  const [currentTodo, setCurrentTodo] = useState({}); 

  //Szükséges, mivel a save button nem a form része, így azon hívva a save-et az event-ből nem kapnánk referenciát a formra => validáció eltörne
  const formRef = useRef();

  useEffect(() => {
    if (props.id)
      setCurrentTodo(state.todos.filter((t) => t.id === props.id)[0]);
    else
      setCurrentTodo({
        columnId: props.columnId,
        deadline: getCurrentDateTimeString(),
      });
  }, [props.show]);

  const handleTitleChange = (event) =>
    setCurrentTodo({ ...currentTodo, title: event.target.value });
  const handleDescriptionChange = (event) =>
    setCurrentTodo({ ...currentTodo, description: event.target.value });
  const handleDeadlineChange = (event) =>
    setCurrentTodo({ ...currentTodo, deadline: event.target.value });
  const handlePriorityChange = (event) =>
    setCurrentTodo({ ...currentTodo, priority: parseInt(event.target.value) });
  const handleStatusChange = (event) =>
    setCurrentTodo({ ...currentTodo, columnId: parseInt(event.target.value) });

  const getCurrentDateTimeString = () => {
    const now = new Date();
    const year = now.getFullYear();
    const month = now.getMonth() >= 10 ? now.getMonth() : `0${now.getMonth()}`;
    const day = now.getDay() >= 10 ? now.getDay() : `0${now.getDay()}`;
    const hours = now.getHours() >= 10 ? now.getHours() : `0${now.getHours()}`;
    const minutes =
      now.getMinutes() >= 10 ? now.getMinutes() : `0${now.getMinutes()}`;
    return `${year}-${month}-${day}T${hours}:${minutes}`;
  };

  const createTodo = async () => {
    const newTodo = await addTodo(currentTodo);
      if(newTodo === null){
        setShowAlert(true);
        setTimeout(() => setShowAlert(false), 3000);
        return null;
      }
      else return [...state.todos, newTodo];
  }

  const modifyTodo = async () => {
    if(!await updateTodo(currentTodo)){
      setShowAlert(true);
      setTimeout(() => setShowAlert(false), 3000);
      return null;
    }
    else{
      const newTodo = await fetchSingleTodo(currentTodo.id);
      return state.todos.map((t) => (t.id === newTodo.id ? newTodo : t));
    }
  }

  const handleSaveOnBackend = async () => {
    let newTodos;
    if (currentTodo.id) newTodos = await modifyTodo();
    else newTodos = await createTodo();

    let isSuccess = true
    if(newTodos === null){
      isSuccess = false;
      newTodos = state.todos;
    }

    dispatch({
      type: 'UPDATE_TODOS',
      payload: {
        todos: newTodos,
      },
    });
    return isSuccess;
  };

  const save = async (event) => {
    event.preventDefault();
    setValidated(true);
    const form = formRef.current;
    if (form.checkValidity() && await handleSaveOnBackend()) {
      props.hide();
      setValidated(false);
    }
  };

  return (
    <>
      <Modal show={props.show} onHide={props.hide}>
        <Modal.Header closeButton>
          <Modal.Title>
            {currentTodo.id ? 'Edit todo' : 'Add a new todo'}
          </Modal.Title>
        </Modal.Header>
        <Modal.Body>
          {/*A teendők rendelkeznek címmel, leírással, határidővel és állapottal (függőben, folyamatban, kész, elhalasztva).*/}
          <Form ref={formRef} noValidate validated={validated} onSubmit={save}>
            <Form.Group as={Row} controlId='title'>
              <Form.Label column md='2'>
                Title
              </Form.Label>
              <Col md='10'>
                <Form.Control
                  required
                  type='text'
                  placeholder='Do homework'
                  value={currentTodo.title ? currentTodo.title : ''}
                  onChange={handleTitleChange}
                />
                <Form.Control.Feedback type='invalid'>
                  Todos must have a title!
                </Form.Control.Feedback>
              </Col>
            </Form.Group>
            <Form.Group as={Row} controlId='description'>
              <Form.Label column md='2'>
                Description
              </Form.Label>
              <Col md='10'>
                <Form.Control
                  value={currentTodo.description ? currentTodo.descriprion : ''}
                  onChange={handleDescriptionChange}
                  as='textarea'
                  rows={2}
                />
              </Col>
            </Form.Group>
            <Form.Group required as={Row} controlId='deadline'>
              <Form.Label column md='2'>
                Deadline
              </Form.Label>
              <Col md='10'>
                <Form.Control
                  value={currentTodo.deadline}
                  onChange={handleDeadlineChange}
                  type='datetime-local'
                  rows={2}
                />
              </Col>
            </Form.Group>
            <Form.Group as={Row} controlId='priority'>
              <Form.Label column md='2'>
                Priority
              </Form.Label>
              <Col md='10'>
                <Form.Control
                  value={currentTodo.priority ? currentTodo.priority : 0}
                  onChange={handlePriorityChange}
                  type='number'
                  min='0'
                  rows={2}
                />
              </Col>
            </Form.Group>
            {currentTodo.id ? (
              <Form.Group as={Row} controlId='status'>
                <Form.Label column md='2'>
                  Status
                </Form.Label>
                <Col md='10'>
                  <Form.Control
                    as='select'
                    onChange={handleStatusChange}
                    value={currentTodo.columnId}
                    rows={2}>
                    {state.columns.map((c) => (
                      <option key={c.id} value={c.id}>
                        {c.title}
                      </option>
                    ))}
                  </Form.Control>
                </Col>
              </Form.Group>
            ) : (
              ''
            )}
          </Form>
          <Alert variant='warning' show={showAlert}>A todo with this name already exists with this title.</Alert>
        </Modal.Body>
        <Modal.Footer>
          <Button variant='secondary' onClick={props.hide}>
            Close
          </Button>
          <Button variant='primary' onClick={save}>
            Save Changes
          </Button>
        </Modal.Footer>
      </Modal>
    </>
  );
}

TodoModal.propTypes = {
  id: PropTypes.number,
  columnId: PropTypes.number.isRequired,
  show: PropTypes.bool.isRequired,
  hide: PropTypes.func.isRequired,
};
