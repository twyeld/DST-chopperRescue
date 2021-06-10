function OnTriggerEnter (info: Collider) {
	Destroy(gameObject);
	ScoreCount.gscore += 1;
}
