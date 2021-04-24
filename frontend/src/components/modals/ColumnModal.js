import PropTypes from 'prop-types';
import React, { useContext, useEffect, useRef, useState } from 'react';
import Alert from 'react-bootstrap/Alert';
import Button from 'react-bootstrap/Button';
import Col from 'react-bootstrap/Col';
import Form from 'react-bootstrap/Form';
import Modal from 'react-bootstrap/Modal';
import Row from 'react-bootstrap/Row';
import { DataContext } from '../../contextProvider/DataContextProvider';
import { addColumn, fetchSingleColumn, updateColumn } from '../../util/columnsApi';

export default function ColumnModal(props) {
  const [currentColumn, setCurrentColumn] = useState({});
  const [validated, setValidated] = useState(false);
  const [showAlert, setShowAlert] = useState(false);
  const [state, dispatch] = useContext(DataContext);
  const formRef = useRef(); //Szükséges, mivel a save button nem a form része, így azon hívva a save-et az event-ből nem kapnánk referenciát a formra => validáció eltörne

  useEffect(() => {
    setCurrentColumn({
      id: props.id,
      title: props.id
        ? state.columns.filter((c) => c.id === props.id)[0].title
        : null,
    });
  }, [props.show]);

  const handleTitleChange = (event) =>
    setCurrentColumn({ ...currentColumn, title: event.target.value });

  const createColumn = async () => {
    const newCol = await addColumn(currentColumn);
    if(newCol === null){
      setShowAlert(true);
      setTimeout(() => setShowAlert(false), 3000);
      return null;
    }
    else return [...state.columns, newCol];
  }
  
  const modifyColumn = async () => {
    const id = currentColumn.id;
    if(!await updateColumn(currentColumn)){
      setShowAlert(true);
      setTimeout(() => setShowAlert(false), 3000);
      return null;
    }
    else{
      const newCol = await fetchSingleColumn(id);
      return state.columns.map((col) => (col.id === id ? newCol : col));
    }
  }

  const handleSaveOnBackend = async () => {
    let newColumns;
    if (currentColumn.id) newColumns = await modifyColumn();
    else newColumns = await createColumn();

    let isSuccess = true
    if(newColumns === null){
      isSuccess = false;
      newColumns = state.columns;
    }
    
    dispatch({
      type: 'UPDATE_COLUMNS',
      payload: {
        columns: newColumns,
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
    <Modal show={props.show} onHide={props.hide}>
      <Modal.Header closeButton>
        <Modal.Title>
          {currentColumn.id ? 'Edit column' : 'Add a new column'}
        </Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <Form ref={formRef} noValidate validated={validated} onSubmit={save}>
          <Form.Group as={Row} controlId='column-name'>
            <Form.Label column md='4'>
              Column name
            </Form.Label>
            <Col md='8'>
              <Form.Control
                required
                type='text'
                placeholder='Done'
                onChange={handleTitleChange}
                value={currentColumn.title ? currentColumn.title : ''}
              />
              <Form.Control.Feedback type='invalid'>
                Columns must have a title!
              </Form.Control.Feedback>
            </Col>
          </Form.Group>
        </Form>
        <Alert show={showAlert} variant='warning'>A column with this name alreay exists.</Alert>
      </Modal.Body>
      <Modal.Footer>
        <Button variant='secondary' onClick={props.hide}>
          Close
        </Button>
        <Button variant='primary' type='submit' onClick={save}>
          Save Changes
        </Button>
      </Modal.Footer>
    </Modal>
  );
}

ColumnModal.propTypes = {
  id: PropTypes.number,
  show: PropTypes.bool.isRequired,
  hide: PropTypes.func.isRequired,
};
