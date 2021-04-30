import baseUrl from './baseUrl';


export async function fetchColumns() {
  const response = await fetch(`${baseUrl}/columns`);
  return await response.json();
}

export async function fetchSingleColumn(columnId) {
  const response = await fetch(`${baseUrl}/columns/${columnId}`);
  return await response.json();
}

export async function addColumn(column) {
  const response = await fetch(`${baseUrl}/columns/`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(column),
  });
  return response.status === 409 ? null : await response.json();
}

export async function updateColumn(column) {
  const response = await fetch(`${baseUrl}/columns/${column.id}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(column),
  });
  return response.status === 409 ? false : true;
}

export async function deleteColumn(columnId) {
  await fetch(`${baseUrl}/columns/${columnId}`, {
    method: 'DELETE',
  });
}