export const DataReducer = (state, action) => {
  switch (action.type) {
    case 'UPDATE_DATA':
      return { ...action.payload };
    case 'UPDATE_COLUMNS':
      return {
        ...state,
        columns: action.payload.columns,
      };
    case 'UPDATE_TODOS':
      return {
        ...state,
        todos: action.payload.todos.sort((a, b) =>
          a.priority < b.priority ? -1 : 1
        ),
      };
    default:
      break;
  }
};
