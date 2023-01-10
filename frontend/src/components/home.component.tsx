import { ChangeEvent, Component } from "react";
import { number } from "yup/lib/locale";
import { ITask } from "../interfaces";

import UserService from "../services/user.service";
import TodoTask from "./TodoTask";

type Props = {};

type State = {
  content: any[];
  task: string;
};

export default class Home extends Component<Props, State> {
  constructor(props: Props) {
    super(props);

    this.state = {
      content: [],
      task: "",
    };
  }
  GetTodos = () => {
    UserService.getUserTodos().then(
      (response) => {
        this.setState({
          content: response.data.map((data: any, index: number) => {
            return { taskName: data.title, id: data.id };
          }),
          task: ""
        });
      },
      (error) => {
        localStorage.clear();
        window.location.href = "/login";
        this.setState({
          content:
            (error.response && error.response.data) ||
            error.message ||
            error.toString(),
        });
      }
    );
  };
  completeTask = (taskid: number): void => {
    UserService.deleteTask(taskid);
    this.setState({
      content: [
        ...this.state.content.filter((todo, index) => {
          return todo.id != taskid;
        }),
      ],
    });
  };

  addTask = (): void => {
    const newTask = this.state.task;
    UserService.addTodo(newTask).then((response) => {
      this.GetTodos();
    });
  };
  editTask = (id: number, task: string): void => {
    UserService.editTodo(id, task).then((response) => {
      this.GetTodos();
    });
  };
  handleChange = (event: ChangeEvent<HTMLInputElement>): void => {
    this.setState({ task: event.target.value });
  };
  componentDidMount() {
    this.GetTodos();
  }

  render() {
    return (
      <div className="container">
        <header className="jumbotron">
          <div className="input-group mb-3">
            <input
              name="task"
              value={this.state.task}
              onChange={this.handleChange}
              type="text"
              className="form-control"
              placeholder="Task title"
              aria-label="Recipient's username"
              aria-describedby="basic-addon2"
            />
            <div className="input-group-append">
              <button
                className="btn btn-outline-secondary"
                onClick={this.addTask}
                type="button"
              >
                Add task
              </button>
            </div>
          </div>
          <hr className="hr mb-5" />

          {this.state.content.map((data, index) => {
            return (
              <TodoTask
                editTask={this.editTask}
                task={data}
                key={data.id}
                completeTask={this.completeTask}
              />
            );
          })}
        </header>
      </div>
    );
  }
}
