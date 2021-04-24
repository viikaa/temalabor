import PropTypes from 'prop-types';
import React, { createContext, useReducer } from 'react';
import { DataReducer } from '../contextProvider/DataReducer';

const initialState = {
  columns: [],
  todos: [],
};

export const DataContext = createContext(initialState);

export const DataContextProvider = (props) => {
  const [columns, dispatch] = useReducer(DataReducer, initialState);

  return (
    <DataContext.Provider value={[columns, dispatch]}>
      {props.children}
    </DataContext.Provider>
  );
};

DataContextProvider.propTypes = {
  children: PropTypes.node,
};
