using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Item : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler,
    IEndDragHandler {
    private Item collisionItem;

    private Vector3 defaultPos;
    protected bool isDrag;

    public bool isMoveable;
    protected bool isOnPointer;
    public string type;

    public void OnBeginDrag(PointerEventData eventData) {
        isDrag = true;
        OnHaveStart();
    }

    public void OnEndDrag(PointerEventData eventData) {
        isDrag = false;
        OnHaveEnd();
        if (collisionItem && collisionItem.canEffect(GetComponent<Item>())) {
            collisionItem.OnEffect(GetComponent<Item>());
        }

        transform.position = defaultPos;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        isOnPointer = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        isOnPointer = false;
    }

    public void Start() {
        defaultPos = gameObject.transform.position;
    }

    public void Update() {
        if (isMoveable && isDrag) {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            gameObject.transform.position = new Vector3(pos.x, pos.y, gameObject.transform.position.z);
            OnHaveStay();
        }
    }

    public void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.GetComponent<Item>()) {
            collisionItem = collider.gameObject.GetComponent<Item>();
        }
    }

    public void OnTriggerExit2D(Collider2D collision) {
        collisionItem = null;
    }

    protected virtual void OnPointerEnter() {
    }

    protected virtual void OnPointerStay() {
    }

    protected virtual void OnPointerExit() {
    }

    protected virtual void OnHaveStart() {
    }

    protected virtual void OnHaveStay() {
    }

    protected virtual void OnHaveEnd() {
    }

    protected abstract void OnEffect(Item dropItem);
    protected abstract bool canEffect(Item dropItem);
}