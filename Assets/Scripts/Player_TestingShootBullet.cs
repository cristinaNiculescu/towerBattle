using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player_TestingShootBullet : NetworkBehaviour {
    
    public GameObject bulletPrefab = null;

    void Update()
    {
        // ... same as before
        if (!isLocalPlayer)
            return;
        // when pressing the space button, ask the server to spawn a bullet
        if (Input.GetKeyDown(KeyCode.Space))
            Cmd_Shoot();
    }

    [Command]
    public void Cmd_Shoot()
    {
        // create server-side instance
        GameObject obj = (GameObject)Instantiate(bulletPrefab, new Vector3(Random.Range(-15f, 15f), 1f, Random.Range(-15f, 15)), Quaternion.identity);
        // setup bullet component
        TestingSpawnBullet bullet = obj.GetComponent<TestingSpawnBullet>();
        bullet.velocity = transform.forward;
        // destroy after 2 secs
        Destroy(obj, 2.0f);
        // spawn on the clients
        NetworkServer.Spawn(obj);
    }
}
