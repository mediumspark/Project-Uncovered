﻿using System.Collections;
using UnityEngine; 

public class TheMayorBoss : Boss
{
    #region Variables
    private GameObject AttackPillar, ProjectileAttack; 
    
    [SerializeField]
    private GameObject SpawnSpotOnPlayer, SpawnSpotAsProjectile;

    private DestructablePilar DestructablePilar;

    [SerializeField]
    private float MayorPilarHeight = 10f;

    [SerializeField]
    private float AttackPilarScale;

    public Vector3 PilarPosition = Vector3.zero;

    bool RecentlyRaisedPilar; 

    bool isPilarAlive = false;

    #endregion

    protected class MayorPilars : MonoBehaviour
    {
        public bool Projectile;
        private float YDestination, RaisingSpeed, MovingSpeed;

        public void SetPilarHeight(float ScaleY, float Speed)
        {
            YDestination = ScaleY; RaisingSpeed = Speed; 
        }
        public void SetPilarSpeed(float speed) { MovingSpeed = speed; }

        private void FixedUpdate()
        {
            if (transform.localScale.y <= YDestination)
            {
                transform.localScale = new Vector3(1, transform.localScale.y + RaisingSpeed, 1);
            }
            if (Projectile)
            {
                transform.Translate(-1 * MovingSpeed, 0, 0);
            }
        }
    }

    public override void OnTakeDamage(int damage)
    {
        base.OnTakeDamage(damage);
    }

    protected override void OnDeath()
    {
        base.OnDeath();
    }

    protected override void Awake()
    {
        base.Awake();

        MaxHealth = 50;
        CurrentHealth = MaxHealth;
        AttackPillar = (GameObject)Resources.Load("Prefabs/Enemies/Boss Attack Pillar");
        ProjectileAttack = (GameObject)Resources.Load("Prefabs/Enemies/Enemy Bullet");
        PrizeWeapon = new TheMayor();

        DestructablePilar = FindObjectOfType<DestructablePilar>();
        DestructablePilar.SetPilarHeight(MayorPilarHeight, 0.25f);
    }

    private void Update()
    {
        SpawnSpotOnPlayer.transform.position = new Vector3(Player.Position.x, transform.parent.position.y);
        isPilarAlive = !DestructablePilar.DeadPilar;
    }


    public void RaiseDestructablePilar()
    {
        DestructablePilar.DeadPilar = false; 
    }

    public void ShootAtPlayer()
    {
        AimedBullet bul = Instantiate(ProjectileAttack, SpawnSpotAsProjectile.transform).AddComponent<AimedBullet>();
        bul.SetDestination(2.0f); 
        bul.BulletSpeed = 2.5f;
        bul.transform.SetParent(null); 
    }

    private void PilarAttack()
    {
        GameObject go = Instantiate(AttackPillar, SpawnSpotOnPlayer.transform.position, Quaternion.identity);
        MayorPilars attack = go.AddComponent<MayorPilars>();
        attack.SetPilarHeight(Player.Position.y + 5.0f, 0.1f);
        RecentlyRaisedPilar = true; 
    }

    private void PilarWave()
    {
        GameObject go = Instantiate(AttackPillar, SpawnSpotAsProjectile.transform);
        MayorPilars attack = go.AddComponent<MayorPilars>();
        attack.SetPilarHeight(3.0f, 0.1f);
    }

    protected override IEnumerator Phase1Attack()
    {
        if (isPilarAlive && !RecentlyRaisedPilar)
        {
            PilarAttack();
            yield return new WaitForSecondsRealtime(BaseCooldownTime);
            ShootAtPlayer(); 
            Destroy(FindObjectOfType<MayorPilars>().gameObject);
            RecentlyRaisedPilar = false; 
        }
        else
        {
            yield return new WaitForSecondsRealtime(BaseCooldownTime * 2);
            RaiseDestructablePilar();
        }
        yield return new WaitForSeconds(BaseCooldownTime);//Attack Over
        startAttack = true;

    }// While on the pillar 1 throwing projectiles

    protected override IEnumerator Phase2Attack()
    {
        if (isPilarAlive)
        {
            PilarAttack();
            yield return new WaitForSecondsRealtime(BaseCooldownTime);
            Destroy(SpawnSpotOnPlayer.transform.GetChild(0).gameObject);
        }
        else
        {
            yield return new WaitForSecondsRealtime(BaseCooldownTime);
            RaiseDestructablePilar();
        }
        yield return new WaitForSeconds(BaseCooldownTime);//Attack over
        startAttack = true;

    }// Summons an even higher pilar
     // While on pillar 2 thowing projectiles, and summoning pilars from the ground

    protected override IEnumerator Phase3Attack()
    {
        PilarWave();
        Destroy(DestructablePilar);
        yield return new WaitForSeconds(BaseCooldownTime);//Attack over
        startAttack = true;
    }
    //knocked off of pilar in one place
    //Jumps around creating waves of pillars 
}