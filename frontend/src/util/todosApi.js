import baseUrl from './baseUrl';

export async function fetchTodos(colId) {
    const url = new URL(`${baseUrl}/todos`, 'http://localhost:5000');
    if (colId) {
      const params = { columnId: colId };
      url.search = new URLSearchParams(params).toString();
    }
    const response = await fetch(url);
    return (await response.json()).sort((a, b) =>
      a.priority < b.priority ? -1 : 1
    );
  }
  
  export async function fetchSingleTodo(todoId) {
    const response = await fetch(`${baseUrl}/todos/${todoId}`);
    return await response.json();
  }
  
  export async function addTodo(todo) {
    const response = await fetch(`${baseUrl}/todos`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(todo),
    });
    return response.json();
  }
  
  export async function updateTodo(todo) {
    fetch(`${baseUrl}/todos/${todo.id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(todo),
    });
  }
  
  export async function deleteTodo(todoId) {
    await fetch(`${baseUrl}/todos/${todoId}`, {
      method: 'DELETE',
    });
  }