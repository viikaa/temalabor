import PropTypes from 'prop-types';
import React from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';

export default function DeleteModal(props) {
  const itemToDelete = `"${props.title}" ${props.itemType}`;

  return (
    <Modal show={props.show} onHide={props.hide}>
      <Modal.Header closeButton>
        <Modal.Title>
          Are you sure you want to delete {itemToDelete}?
        </Modal.Title>
      </Modal.Header>
      <Modal.Footer>
        <Button variant='secondary' onClick={props.hide}>
          Close
        </Button>
        <Button variant='danger' type='submit' onClick={props.delete}>
          Delete
        </Button>
      </Modal.Footer>
    </Modal>
  );
}

DeleteModal.propTypes = {
  title: PropTypes.string.isRequired,
  itemType: PropTypes.oneOf(['todo', 'column']),
  show: PropTypes.bool.isRequired,
  hide: PropTypes.func.isRequired,
  delete: PropTypes.func.isRequired,
};
