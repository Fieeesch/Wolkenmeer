using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

[RequireComponent(typeof(LineRenderer))]
public class EnemyMovementBezier : MonoBehaviour
{
    [Header("Ziel-Objekt (Schiff)")]
    public Transform target;
    
    [Header("Allgemeine Flug-Parameter")]
    [Tooltip("Basis-Vorwärts-Geschwindigkeit (positiv).")]
    public float baseSpeed = 1f / 60f;
    
    [FormerlySerializedAs("speed_multiplier")] public float speedMultiplier = 1f;

    public float randomHeightMin = 5f;
    public float randomHeightMax = 10f;
    public float horizontalRange = 3f;

    public bool KnockBack { get; private set; }

    private LineRenderer _lineRenderer;

    // Bezier-Punkte
    private Vector3 _p0, _p1, _p2, _p3;
    
    // t auf [0..1]
    private float _paramT;
    private bool _isFlying;

    // Liste für aktive Knockback-Effekte
    private readonly List<KnockbackEffect> _knockbackEffects = new List<KnockbackEffect>();

    // Klasse, die einen Knockback-Effekt repräsentiert
    private class KnockbackEffect
    {
        private readonly float _initialValue;
        
        public readonly float Duration;
        public float Elapsed;

        public KnockbackEffect(float initialValue, float duration)
        {
            _initialValue = initialValue;
            Duration = duration;
            Elapsed = 0f;
        }

        // Quadratische Interpolation: Von initialValue bis 0
        public float GetCurrentValue()
        {
            var t = Mathf.Clamp01(Elapsed / Duration);
            return Mathf.Lerp(_initialValue, 0f, t * t);
        }
    }

    private void Awake()
    {
        // Stelle sicher, dass ein LineRenderer vorhanden ist
        _lineRenderer = GetComponent<LineRenderer>();
    }

    /// <summary>
    /// Initialisiert die Bezier-Kurve und startet den Flug.
    /// </summary>
    /// <param name="spawnPosition">Startpunkt (wird zu p0)</param>
    /// <param name="targetTransform">Ziel-Transform (wird zu p3)</param>
    public void InitializeMovement(Vector3 spawnPosition, Transform targetTransform)
    {
        _p0 = spawnPosition;
        transform.position = _p0;

        target = targetTransform;
        _p3 = target.position;

        // Kontrollpunkte berechnen
        _p1 = CalculateControlPoint1(_p0, _p3);
        _p2 = CalculateControlPoint2(_p0, _p3);

        // paramT zurücksetzen und Flug starten
        _paramT = 0f;
        _isFlying = true;
        
        // DrawBezierCurve();
    }

    private void Update()
    {
        if (!_isFlying) return;

        // Update der aktiven Knockback-Effekte und Aufsummierung ihrer Beiträge
        var knockbackOffset = 0f;
        for (var i = _knockbackEffects.Count - 1; i >= 0; i--)
        {
            var effect = _knockbackEffects[i];
            effect.Elapsed += Time.deltaTime;
            if (effect.Elapsed >= effect.Duration)
            {
                _knockbackEffects.RemoveAt(i);
            }
            else
            {
                knockbackOffset += effect.GetCurrentValue();
            }
        }
        KnockBack = knockbackOffset != 0f;
        
        // Effektive Geschwindigkeit = Basisgeschwindigkeit + Summe der Knockback-Effekte
        var effectiveSpeed = baseSpeed + knockbackOffset;

        // Erhöhe paramT anhand der effektiven Geschwindigkeit
        _paramT += effectiveSpeed * speedMultiplier * Time.deltaTime;
        _paramT = Mathf.Clamp01(_paramT);

        // Bestimme die aktuelle Position auf der Bezier-Kurve
        var bezierPos = CubicBezier(_p0, _p1, _p2, _p3, _paramT);
        transform.position = bezierPos;

        // Richte das Objekt in Flugrichtung aus
        if (_paramT < 1f)
        {
            var smallStep = Mathf.Clamp01(_paramT + 0.01f);
            var futurePos = CubicBezier(_p0, _p1, _p2, _p3, smallStep);
            var direction = (futurePos - transform.position).normalized;
            if (direction.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
        }
        else
        {
            // Flug beendet
            _isFlying = false;
        }
    }

    /// <summary>
    /// Zeichnet die Bezier-Kurve im LineRenderer (optional fürs Debugging).
    /// </summary>
    private void DrawBezierCurve()
    {
        if (!_lineRenderer) return;

        const int resolution = 20;
        _lineRenderer.positionCount = resolution + 1;

        for (var i = 0; i <= resolution; i++)
        {
            var t = i / (float)resolution;
            var pointOnCurve = CubicBezier(_p0, _p1, _p2, _p3, t);
            _lineRenderer.SetPosition(i, pointOnCurve);
        }
    }

    /// <summary>
    /// Berechnet den ersten Kontrollpunkt: relativ zufällig über dem Spawn.
    /// </summary>
    private Vector3 CalculateControlPoint1(Vector3 spawnPos, Vector3 targetPos)
    {
        var cp1 = spawnPos;
        var randomHeight = Random.Range(randomHeightMin, randomHeightMax);
        cp1 += Vector3.up * randomHeight;
        cp1 += new Vector3(Random.Range(-horizontalRange, horizontalRange),
                           0f,
                           Random.Range(-horizontalRange, horizontalRange));
        return cp1;
    }

    /// <summary>
    /// Berechnet den zweiten Kontrollpunkt: ungefähr in der Mitte, leicht versetzt.
    /// </summary>
    private Vector3 CalculateControlPoint2(Vector3 spawnPos, Vector3 targetPos)
    {
        var midpoint = Vector3.Lerp(spawnPos, targetPos, 0.5f);
        var randomHeight = Random.Range(randomHeightMin * 0.5f, randomHeightMax * 0.5f);
        var cp2 = midpoint + Vector3.up * randomHeight;
        cp2 += new Vector3(Random.Range(-horizontalRange, horizontalRange),
                           0f,
                           Random.Range(-horizontalRange, horizontalRange));
        return cp2;
    }

    /// <summary>
    /// Standardformel für eine kubische Bezier-Kurve.
    /// </summary>
    private Vector3 CubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        var u = 1f - t;
        var tt = t * t;
        var uu = u * u;
        var uuu = uu * u;
        var ttt = tt * t;

        var p = uuu * p0;           // (1-t)^3 * p0
        p += 3f * uu * t * p1;          // 3 * (1-t)^2 * t * p1
        p += 3f * u * tt * p2;          // 3 * (1-t) * t^2 * p2
        p += ttt * p3;                  // t^3 * p3
        return p;
    }

    /// <summary>
    /// Fügt einen Knockback-Effekt hinzu, der sich quadratisch zur Basisgeschwindigkeit zurückinterpoliert.
    /// Negative Werte bewirken eine Rückwärtsbewegung.
    /// </summary>
    /// <param name="negativeKnockback">Negativer Wert, z. B. -5f</param>
    /// <param name="duration">Dauer in Sekunden</param>
    public void KnockbackQuadratic(float negativeKnockback, float duration)
    {
        // Neuer Effekt wird zur Liste hinzugefügt.
        _knockbackEffects.Add(new KnockbackEffect(negativeKnockback, duration));
    }
}
