import PropTypes from 'prop-types';
import React, { useState } from 'react';
import Button from 'react-bootstrap/Button';
import ColumnModal from './modals/ColumnModal';
import TodoModal from './modals/TodoModal';

export default function Placeholder(props) {
  const [show, setShow] = useState(false);

  const handleClose = () => setShow(false);
  const handleShow = () => setShow(true);

  const placeHolderClass = `py-3 ${props.type}-placeholder`;

  let modal;
  if (props.type === 'todo')
    modal = (
      <TodoModal
        show={show}
        hide={handleClose}
        columnId={props.columnId ? props.columnId : 0}
      />
    );
  //ESLint warning miatt így jobb
  else modal = <ColumnModal show={show} hide={handleClose} />;

  return (
    <>
      <Button
        variant='outline-light'
        block
        className={placeHolderClass}
        onClick={handleShow}>
        <i className='fas fa-plus-circle'></i> Add a new {props.type}
      </Button>
      {modal}
    </>
  );
}

Placeholder.propTypes = {
  type: PropTypes.oneOf(['todo', 'column']).isRequired,
  columnId: PropTypes.number,
};
