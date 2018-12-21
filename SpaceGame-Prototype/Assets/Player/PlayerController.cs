using System;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    public float ThrustSpeed = 150.0f;
    public float RotationSpeed = 3.0f;

    public GameObject PlayerCameraPrefab;

    private readonly GameObject ingameCameraReference;

    private GameObject ServerBullet;
    public void Start()
    {
        if (isServer)
        {
           // Create bullet in object-pool
           ServerBullet = (GameObject)Instantiate(
           BulletPrefab,
           new Vector3(1000, 1000, 0),
           new Quaternion());
        }
    }

    public override void OnStartLocalPlayer()
    {
        // Create new Player-Camera
        var camera = (GameObject)Instantiate(PlayerCameraPrefab, new Vector3(0, 0, -10), new Quaternion());

        // Set it to follow Player-Transform
        camera.GetComponent<CameraFollow>().Player = transform;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        // Get inpput
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * -ThrustSpeed;
        var y = Input.GetAxis("Vertical") * Time.deltaTime * RotationSpeed;

        // Move Player
        transform.Rotate(0, 0, x);
        transform.Translate(0, y, 0);

        if (Input.GetKeyDown(KeyCode.Space))
        {         
            CmdFirePool(BulletSpeed, BulletLifeTime, BulletSpawn.position, BulletSpawn.rotation);
        }
    }

    public GameObject BulletPrefab;
    public Transform BulletSpawn;
    public float BulletSpeed = 8;
    public int BulletLifeTime = 2;

    /// <summary>
    /// Simple CPU-expensive Fire-Function, noticeable delay in clients 
    /// </summary>
    /// <param name="bulletSpeed"></param>
    /// <param name="bulletLifeTime"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    [Command]
    private void CmdFire(float bulletSpeed, float bulletLifeTime, Vector3 position, Quaternion rotation)
    {
        // Create the bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(
            BulletPrefab,
            position,
            rotation);

        // Add the velocity to the bullet
        bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * bulletSpeed;

        // SPawn the bullet on the Clients
        NetworkServer.Spawn(bullet);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, bulletLifeTime);
    }
    [Command]
    private void CmdFirePool(float bulletSpeed, float bulletLifeTime, Vector3 position, Quaternion rotation)
    {
        ServerBullet.transform.position = position;
        ServerBullet.transform.rotation = rotation;

        // Add the velocity to the bullet
        ServerBullet.GetComponent<Rigidbody2D>().velocity = ServerBullet.transform.up * bulletSpeed;

        // SPawn the bullet on the Clients
        NetworkServer.Spawn(ServerBullet);
    }

}
