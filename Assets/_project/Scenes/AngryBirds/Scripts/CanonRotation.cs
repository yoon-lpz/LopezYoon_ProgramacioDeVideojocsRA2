using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;

public class CanonRotation : MonoBehaviour, InputSystem_Actions.ICannonActions
{
    public Vector3 _maxRotation;
    public Vector3 _minRotation;
    private float _offset = -51.6f;
    public GameObject ShootPoint;
    public GameObject Bullet;
    public float ProjectileSpeed = 0;
    public float MaxSpeed;
    public float MinSpeed;
    public GameObject PotencyBar;
    private float _initialScaleX;
    private Vector2 _distanceBetweenMouseAndPlayer;
    private bool isRaising = false;
    [SerializeField] private float _multiplier = 10f;

	[SerializeField] private Camera MainCamera;
    public InputSystem_Actions InputAction;
	private void Awake()
    {
        InputAction = GetComponent<InputSystem_Actions>();
        InputAction.Cannon.SetCallbacks(this);
        _initialScaleX = PotencyBar.transform.localScale.x;
    }

    public void OnEnable() => InputAction.Enable();
    public void OnDisable() => InputAction.Enable();

	void Update()
    {
        if (isRaising)
        {
            ProjectileSpeed = Time.deltaTime * _multiplier + ProjectileSpeed; //acotar entre dos valors (mirar variables)
            ProjectileSpeed = Mathf.Clamp(ProjectileSpeed, MinSpeed, MaxSpeed);
            CalculateBarScale();
        }
        
        CalculateBarScale();

    }
    public void CalculateBarScale()
    {
        PotencyBar.transform.localScale = new Vector3(Mathf.Lerp(0, _initialScaleX, ProjectileSpeed / MaxSpeed),
            transform.localScale.y,
            transform.localScale.z);
    }
    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isRaising = true;
        }
        if (context.canceled)
        {
            var projectile = Instantiate(Bullet, transform.position, Quaternion.identity); //canviar la posició on s'instancia
            projectile.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(ProjectileSpeed, ProjectileSpeed);

            ProjectileSpeed = 0f;
			isRaising = false;
        }
    }

    public void OnAim(InputAction.CallbackContext context) {
        var screenPosition = context.ReadValue<Vector2>();
		Vector3 mousePos = MainCamera.ScreenToWorldPoint(screenPosition); //obtenir el valor del click del cursor (Fer amb new input system)
		_distanceBetweenMouseAndPlayer = mousePos - transform.position; //obtenir el vector distància entre el canó i el cursor
		float angle = (Mathf.Atan2(_distanceBetweenMouseAndPlayer.y, _distanceBetweenMouseAndPlayer.x) * 180f / Mathf.PI + _offset);
        angle = Mathf.Clamp(angle, 0, 90);
        transform.rotation = Quaternion.Euler(0, 0, angle); //en quin dels tres eixos va l'angle?
	}
}
