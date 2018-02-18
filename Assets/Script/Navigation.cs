using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class Node {
	public double cost;
	public double heuristicCost;
	public double score;
	public PassConnect passConnect;
	public Node parent;
	public Node(double cost, PassConnect goal, Node parent, PassConnect passConnect) {
		this.cost = cost;
		this.passConnect = passConnect;
		this.parent = parent;

		this.heuristicCost = Vector2.Distance(goal.owner.position, passConnect.owner.position);
		this.score = this.cost + this.heuristicCost;
	}
}

class Navigation {
	private List<Node> openList = new List<Node>();
	private List<Node> closeList = new List<Node>();
	private PassConnect startPass;
	private PassConnect goalPass;

	public Navigation(Vector2 startPos, Vector2 goalPos) {
		startPass = getPosUnderPass(startPos);
		goalPass = getPosUnderPass(goalPos);
	}
	public List<PassConnect> getRoute() {
		if(startPass == goalPass){
			var r = new List<PassConnect>();
			r.Add(goalPass);
			return r;
		}

		List<PassConnect> route = new List<PassConnect>();
		var goalNode = new Node(0, startPass, null, goalPass);
		closeList.Add(goalNode);
		aroundOpen(goalNode);
		Node currentNode = searchMinScoreOpenNode();
		while (currentNode.passConnect != startPass) {
			aroundOpen(currentNode);
			closeNode(currentNode);
			if (openList.Count == 0) {
				return null;
			}

			currentNode = searchMinScoreOpenNode();
		}
		while (currentNode != null) {
			route.Add(currentNode.passConnect);
			currentNode = currentNode.parent;
		}

		return route;
	}
	private void aroundOpen(Node passNode) {
		foreach (var aroundPass in passNode.passConnect.passConnectList) {
			if (
				openList.Find(x => x.passConnect == aroundPass) != null ||
				closeList.Find(x => x.passConnect == aroundPass) != null
			) {
				continue;
			}
			double cost = Vector2.Distance(passNode.passConnect.owner.position, aroundPass.owner.position) + passNode.cost;
			openList.Add(new Node(cost, startPass, passNode, aroundPass));
		}
	}
	private Node searchMinScoreOpenNode() {
		return openList.Aggregate((c, n) => c.score < n.score ? c : n);
	}
	private void closeNode(Node node) {
		openList.Remove(node);
		closeList.Add(node);
	}
	static public PassConnect getPosUnderPass(Vector2 pos) {
		var list = Physics2D.RaycastAll(pos, Vector2.down);
		foreach (var v in list) {
			var passConnect = v.transform.GetComponent<PassConnect>();
			if (passConnect) {
				return passConnect;
			}
		}
		return null;
	}
}