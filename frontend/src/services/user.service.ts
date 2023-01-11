import axios from 'axios';
import { ITask } from '../types/interfaces';

const API_URL = 'https://localhost:7080/api/';
const userStr = localStorage.getItem("user");
let user: any = null;
let config: any = null;
if (userStr) {
  user = JSON.parse(userStr)
  config = { 
    headers: { Authorization: `Bearer ${user.accessToken}` }
  }
}
class UserService {
  getUserTodos() {
    return axios.post(API_URL + 'Tasks/GetAll?id=' + user.id, null, config);
  }

  addTodo(task: ITask) {
    return axios.post(API_URL + 'Tasks/Add?id=' + user.id, {
      title: task.taskName,
    }, config);
  }
  editTodo(id: number ,task: ITask) {
    return axios.post(API_URL + 'Tasks/Update?id=' + id, {
      title: task.taskName,
    }, config);
  }

  deleteTask(id: number) {
    return axios.post(API_URL + "Tasks/Delete?id=" + id, null, config )
  }
}

export default new UserService();