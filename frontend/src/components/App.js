import React, { useContext, useEffect } from 'react';
import Col from 'react-bootstrap/Col';
import Container from 'react-bootstrap/Container';
import Navbar from 'react-bootstrap/Navbar';
import Row from 'react-bootstrap/Row';
import { DataContext } from '../contextProvider/DataContextProvider';
import '../style/App.css';
import { fetchColumns } from '../util/columnsApi';
import { fetchTodos } from '../util/todosApi';
import Column from './Column';
import Placeholder from './Placeholder';

function App() {
  const [state, dispatch] = useContext(DataContext);

  const initColumns = async () => {
    const columnData = await fetchColumns();
    const todoData = await fetchTodos();
    dispatch({
      type: 'UPDATE_DATA',
      payload: {
        columns: columnData,
        todos: todoData,
      },
    });
  };

  useEffect(() => {
    initColumns();
  }, []);

  return (
    <div className='App d-flex flex-column'>
      <Navbar bg='dark' variant='dark'>
        <Navbar.Brand>TODO</Navbar.Brand>
      </Navbar>
      <Container fluid className='p-0 d-flex flex-column flex-grow-1'>
        <Row className='flex-nowrap overflow-auto m-0 pt-4 flex-grow-1 '>
          {state.columns.length ? (
            <>
              {state.columns.map((column) => (
                <Column key={column.id} id={column.id} />
              ))}
            </>
          ) : (
            ''
          )}
          <Col lg='3' md='4' sm='6'>
            <Placeholder type='column' />
          </Col>
        </Row>
      </Container>
    </div>
  );
}

export default App;
