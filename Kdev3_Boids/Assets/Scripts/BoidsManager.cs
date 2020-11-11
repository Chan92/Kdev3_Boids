using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsManager:MonoBehaviour {
	public int boidAmount = 10;
	public GameObject boidPrefab;
	public Vector3 fieldSize = new Vector3(17f, 10f, 5f);

	[Space(20)]
	public bool useCohesion = true;
	public bool useSeperation = true;
	public bool useAlignment = true;

	[Space(10)]
	public bool nearNeighboursOnly;
	public float nearDistance = 10f;

	[Space(10)]
	public float r1 = 100f;
	public float r2 = 100f;
	public float r3 = 8f;

	[Space(10)]
	public float cohesionSpeed = 0.5f;
	public float alignmentSpeed = 0.5f;

	public List<Boid> boids = new List<Boid>();

	void Start() {
		CreateBoids();
	}

	void Update() {
		UpdateBoids();
	}

	void CreateBoids() {
		for(int i = 0; i < boidAmount; i++) {
			GameObject newBoid = Instantiate(boidPrefab);
			Boid b = newBoid.GetComponent<Boid>();

			float randomX = Random.Range(-fieldSize.x, fieldSize.x);
			float randomY = Random.Range(-fieldSize.y, fieldSize.y);
			float randomZ = Random.Range(0, fieldSize.z);
			newBoid.transform.position = new Vector3(randomX, randomY, randomZ);

			b.velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
			//print("created /w velocity: " + b.velocity);

			newBoid.name = "Boid " + i;
			boids.Add(b);
		}
	}

	void UpdateBoids() {
		foreach(Boid b in boids) {
			b.velocity = Vector3.zero;
			if(useCohesion)
				b.velocity += Cohesion(b);
			if(useSeperation)
				b.velocity += Seperation(b);
			if(useAlignment)
				b.velocity += Alignment(b);

			Vector3 pos = b.transform.position + b.velocity;
			b.transform.position = CheckFieldPosition(pos);
		}
	}

	//Rule1: fly towards center of mass of neighbouring boids
	public Vector3 Cohesion(Boid boidJ) {
		Vector3 pc = Vector3.zero;
		int counter = 0;

		foreach(Boid b in boids) {
			if(b != boidJ) {
				if(nearNeighboursOnly) {
					if(Vector3.Distance(b.transform.position, boidJ.transform.position) < nearDistance) {
						pc += b.transform.position;
						counter++;
					}
				} else {
					pc += b.transform.position;
					counter++;
				}
			}
		}

		if(counter > 0) {
			pc /= counter;
			pc = pc.normalized * cohesionSpeed;
			pc = (pc - boidJ.transform.position) / r1;
		}

		//print(boidJ.gameObject.name + " - Rule1: " + pc);
		return pc;
	}

	//Rule2: keep small distance of other objects (boids included)
	public Vector3 Seperation(Boid boidJ) {
		Vector3 c = Vector3.zero;

		foreach(Boid b in boids) {
			if(b != boidJ) {
				if(Vector3.Distance(b.transform.position, boidJ.transform.position) < r2) {
					c = c - (b.transform.position - boidJ.transform.position);
				}
			}
		}

		//print(boidJ.gameObject.name + " - Rule2: " + c);
		return c;
	}

	//Rule3: boids try to match velocity with nearby boids
	public Vector3 Alignment(Boid boidJ) {
		Vector3 pv = Vector3.zero;
		int counter = 0;

		foreach(Boid b in boids) {
			if(b != boidJ) {
				if(nearNeighboursOnly) {
					if(Vector3.Distance(b.transform.position, boidJ.transform.position) < nearDistance) {
						pv += b.velocity;
						counter++;
					}
				} else {
					pv += b.velocity;
					counter++;
				}
			}
		}

		if(counter > 0) {
			pv /= counter;
			pv = pv.normalized * alignmentSpeed;
			pv = (pv - boidJ.velocity) / r3;
		}

		//print(boidJ.gameObject.name + " - Rule3: " + pv);
		return pv;
	}

	public Vector3 CheckFieldPosition(Vector3 p) {
		float offset = 0.5f;

		if(p.x < -fieldSize.x) {
			p.x = fieldSize.x - offset;
		} else if(p.x > fieldSize.x) {
			p.x = -fieldSize.x + offset;
		}

		if(p.y < -fieldSize.y) {
			p.y = fieldSize.y - offset;
		} else if(p.y > fieldSize.y) {
			p.y = -fieldSize.y + offset;
		}

		if(p.z < 0) {
			p.z = fieldSize.z - offset;
		} else if(p.z > fieldSize.z) {
			p.z = 0 + offset;
		}

		return p;
	}
}
