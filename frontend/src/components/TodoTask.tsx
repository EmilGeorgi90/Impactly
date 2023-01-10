import React, { ChangeEvent, useState } from "react";
import { ITask } from "../interfaces";

interface Props {
  task: any;
  completeTask(id: number): void;
  editTask(id: number, task: string): void;
}

const TodoTask = ({ task, completeTask, editTask }: Props) => {
  const [curTask, setCurTask] = useState<string>(task.taskName);
  const handleChange = (event: ChangeEvent<HTMLInputElement>): void => {
    console.log("test")
    setCurTask(event.target.value);
  };
  return (
    <div className="task">
      <div className="input-group mb-3">
        <input
          name="task"
          value={curTask}
          onChange={handleChange}
          type="text"
          className="form-control"
          placeholder="Task title"
          aria-label="Recipient's username"
          aria-describedby="basic-addon2"
        />
        <div className="btn-group" role="group" aria-label="Basic example">
          <button type="button" className="btn btn-secondary" onClick={() => {
          completeTask(task.id);
        }}>
            Delete
          </button>
          <button type="button" className="btn btn-secondary" onClick={() => {
          editTask(task.id, curTask);
        }}>
            Edit
          </button>
        </div>
      </div>
    </div>
  );
};
export default TodoTask;
