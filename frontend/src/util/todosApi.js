import baseUrl from './baseUrl';

export async function fetchTodos() {
  const response = await fetch(`${baseUrl}/todos`);
  return await response.json();
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
  return response.status === 409 ? null : await response.json();
}

export async function updateTodo(todo) {
  const response = await fetch(`${baseUrl}/todos/${todo.id}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(todo),
  });
  return response.status === 409 ? false : true;
}

export async function deleteTodo(todoId) {
  await fetch(`${baseUrl}/todos/${todoId}`, {
    method: 'DELETE',
  });
}