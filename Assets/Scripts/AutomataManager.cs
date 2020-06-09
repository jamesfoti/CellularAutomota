﻿using System;
using UnityEngine;

public class AutomataManager : MonoBehaviour {

	public Color aliveColor;
	public Color deadColor;
	public float colorLerpTime;
	public float randomFillPercent;
	public float smoothIterations;
	public string seed;
	public bool useRandomSeed;

	private GridManager grid;


	private void Awake() {
		grid = GetComponent<GridManager>();
	}

	public void GenerateCave() {
		// This is gonna be used but for different rules other than Game of Life
		PseudoRandFillGrid();

		for (int i = 0; i < smoothIterations; i++) {
			SmoothGrid();
		}
	}

	public void ResetGrid() {
		for (int i = 0; i < grid.listCells.Count; i++) {
			grid.listCells[i].currentState = Cell.States.Dead;
			grid.listCells[i].nextState = Cell.States.Dead;
		}
		UpdateGridColors();
	}

	public void PseudoRandFillGrid() {
		if (useRandomSeed) {
			seed = Time.time.ToString();
		}

		System.Random pseudoRandom = new System.Random(seed.GetHashCode());

		for (int i = 0; i < grid.listCells.Count; i++) {
			if (pseudoRandom.Next(0, 100) < randomFillPercent) {
				grid.listCells[i].currentState = Cell.States.Alive;
			}
			else {
				grid.listCells[i].currentState = Cell.States.Dead;
			}
			
		}
		UpdateGridColors(lerp: false);
	}

	public void RandomFillGrid() {
		for (int i = 0; i < grid.listCells.Count; i++) {
			Array values = Enum.GetValues(typeof(Cell.States));
			Cell.States randomState = (Cell.States)values.GetValue(UnityEngine.Random.Range(0, values.Length));

			grid.listCells[i].currentState = randomState;
			UpdateGridColors();
		}
	}

	public void SmoothGrid() {
		for (int i = 0; i < grid.listCells.Count; i++) {
		
			int livingNeighbors = GetNearbyNeighbors(grid.listCells[i].rowPos, grid.listCells[i].colPos);

			if (grid.listCells[i].currentState == Cell.States.Alive) {
				if (livingNeighbors < 2) {
					grid.listCells[i].nextState = Cell.States.Dead;
				}
				if (livingNeighbors == 2 || livingNeighbors == 3) {
					grid.listCells[i].nextState = Cell.States.Alive;
				}
				if (livingNeighbors > 3) {
					grid.listCells[i].nextState = Cell.States.Dead;
				}
			}
			if (grid.listCells[i].currentState == Cell.States.Dead) {
				if (livingNeighbors == 3) {
					grid.listCells[i].nextState = Cell.States.Alive;
				}
			}
		}

		for (int i = 0; i < grid.listCells.Count; i++) {
			grid.listCells[i].currentState = grid.listCells[i].nextState;
		}

		UpdateGridColors(lerp: true);
	}


	public int GetNearbyNeighbors(int gridRow, int gridCol) {
		int count = 0;
		for (int i = -1; i < 2; i++) {
			for (int j = -1; j < 2; j++) {
				int row = (gridRow + i + grid.numRows) % grid.numRows;
				int col = (gridCol + j + grid.numCols) % grid.numCols;

				if (grid.cells[row,col].currentState == Cell.States.Alive) {
					count++;
				}
			}
		}

		count -= (int)grid.cells[gridRow, gridCol].currentState;

		return count;
	}

	public void UpdateGridColors(bool lerp = false) {
		float lerpTime;
		if (lerp == false) {
			lerpTime = 1f;
		}
		else {
			lerpTime = this.colorLerpTime;
		}

		for (int i = 0; i < grid.listCells.Count; i++) { 
			if (grid.listCells[i].currentState == Cell.States.Alive) {
				grid.listCells[i].render.color = Color.Lerp(grid.listCells[i].render.color, aliveColor, lerpTime);
			}
			else {
				grid.listCells[i].render.color = Color.Lerp(grid.listCells[i].render.color, deadColor, lerpTime);
			}
		}
	}




}