import React, { ChangeEvent, Component } from "react";
import { ITask } from "../types/interfaces";

type Props = {
  task: ITask;
  completeTask(id: number): void;
  editTask(id: number, task: ITask | null): void;
};
type State = {
  curTask: ITask | null;
};

export default class TodoTask extends Component<Props, State> {
  constructor(props: Props) {
    super(props);

    this.state = {
      curTask: null,
    };
  }
  componentDidMount() {
    this.setState({curTask: this.props.task})
  }

  handleChange = (event: ChangeEvent<HTMLInputElement>): void => {
    console.log("test");
    this.setState({ curTask: { taskName: event.target.value, id: Number(event.target.id)}});
  };
  render() {
    return (
      <div className="task">
        <div className="input-group mb-3">
          <input
            name="task"
            value={this.state.curTask != null ? this.state.curTask?.taskName : ""}
            onChange={this.handleChange}
            type="text"
            className="form-control"
            placeholder="Task title"
            aria-label="Recipient's username"
            aria-describedby="basic-addon2"
          />
          <div className="btn-group" role="group" aria-label="Basic example">
            <button
              type="button"
              className="btn btn-secondary"
              onClick={() => {
                this.props.completeTask(this.props.task.id);
              }}
            >
              Delete
            </button>
            <button
              id={this.state.curTask?.id.toString()}
              type="button"
              className="btn btn-secondary"
              onClick={() => {
                this.props.editTask(this.props.task.id, this.state.curTask);
              }}
            >
              Edit
            </button>
          </div>
        </div>
      </div>
    );
  }
}
