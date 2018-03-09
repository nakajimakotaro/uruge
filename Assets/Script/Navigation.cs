using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class Node {
	public double cost;
	public double heuristicCost;
	public double score;
	public Navi navi;
	public Node parent;
	public Node(double cost, Navi goal, Node parent, Navi passConnect) {
		this.cost = cost;
		this.navi = passConnect;
		this.parent = parent;

		this.heuristicCost = Vector2.Distance(goal.position, passConnect.position);
		this.score = this.cost + this.heuristicCost;
	}
}

static class Navigation {
	public static List<Navi> naviList = new List<Navi>();
	
	public static void bake(){
		foreach(var groudArea in naviList){
			groudArea.split();
		}
		foreach(var groudArea in naviList){
			groudArea.connect();
		}
	}
	public static List<Navi> getRoute(Vector2 startPos, Vector2 goalPos) {
		List<Node> openList = new List<Node>();
		List<Node> closeList = new List<Node>();
		Navi startPass;
		Navi goalPass;

		startPass = getPosUnderPass(startPos);
		goalPass = getPosUnderPass(goalPos);

		if (startPass == goalPass) {
			var r = new List<Navi>();
			r.Add(goalPass);
			return r;
		}

		List<Navi> route = new List<Navi>();
		var goalNode = new Node(0, startPass, null, goalPass);
		closeList.Add(goalNode);
		aroundOpen(goalNode, openList, closeList, startPass);
		Node currentNode = searchMinScoreOpenNode(openList);
		while (currentNode.navi != startPass) {
			aroundOpen(currentNode, openList, closeList, startPass);
			closeNode(currentNode, openList, closeList);
			if (openList.Count == 0) {
				return null;
			}

			currentNode = searchMinScoreOpenNode(openList);
		}
		while (currentNode != null) {
			route.Add(currentNode.navi);
			currentNode = currentNode.parent;
		}

		return route;
	}
	private static void aroundOpen(Node passNode, List<Node> openList, List<Node> closeList, Navi startPass) {
		foreach (var aroundPass in passNode.navi.connectNaviList) {
			if (
				openList.Find(x => x.navi == aroundPass) != null ||
				closeList.Find(x => x.navi == aroundPass) != null
			) {
				continue;
			}
			double cost = Vector2.Distance(passNode.navi.position, aroundPass.position) + passNode.cost;
			openList.Add(new Node(cost, startPass, passNode, aroundPass));
		}
	}
	private static Node searchMinScoreOpenNode(List<Node> openList) {
		return openList.Aggregate((c, n) => c.score < n.score ? c : n);
	}
	private static void closeNode(Node node, List<Node> openList, List<Node> closeList) {
		openList.Remove(node);
		closeList.Add(node);
	}
	static Navi getPosUnderPass(Vector2 pos) {
		var list = Physics2D.RaycastAll(pos, Vector2.down);
		foreach (var v in list) {
			var passConnect = v.transform.GetComponent<GroundArea>();
			if (!passConnect) {
				continue;
			}
			var naviArea = passConnect.getNavi(pos.x);
			if (naviArea == null) {
				continue;
			}
			return naviArea;
		}
		return null;
	}
}