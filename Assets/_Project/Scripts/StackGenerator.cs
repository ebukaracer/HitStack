using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-1)]
internal class StackGenerator : MonoBehaviour
{
    public static StackGenerator Instance { get; private set; }

    private readonly string _goodStackTag = "Good";
    private readonly string _badStackTag = "Bad";

    private int _childrenCount;

    private readonly List<GameObject> _parentStacks = new List<GameObject>();
    private GameObject _stackPrefab;

    [SerializeField] private Transform checkpointPipe;
    [SerializeField] private Transform checkpointCylinder;

    [SerializeField] private List<GameObject> pileStacks;

    [Space(10), SerializeField, Tooltip("Any Color can serve as a Bad stack color, Default color is distinguishable")]
    private Material colorBad;

    [SerializeField] private Material colorGood;
    [SerializeField] private Material checkpointMat;

    [Space(10), SerializeField, Tooltip("Minimum amount of stack to be generated")]
    private int minStack = 100;

    [SerializeField, Tooltip("Maximum amount of stack to be generated")]
    private int maxStack = 200;

    [field: SerializeField, Tooltip("Total stacks to be generated, 'min max' is ignored here when working in the editor.")]
    public int GeneratedStackAmount { get; private set; }

    [Space(10), SerializeField, Range(0f, 1f), Tooltip("Too much spacing may cause bounce problems")]
    private float spacing = .51f;

    [SerializeField, Range(0, 2), Tooltip("Too much rotation may cause bounce problems")]
    private float stepAngle = .8f;


    private void Awake()
    {
        Instance = this;

        colorGood.color = ColorGenerator.Instance.GetColor(s: .7f);
        checkpointMat.color = ColorGenerator.Instance.GetColor(v: .65f);

        GeneratedStackAmount = Random.Range(minStack, maxStack);

        GenerateStack();
        AssignRandomMaterial2();
        RefreshCheckpoint();

    }

    public void GenerateStack()
    {
        // Selects a random stack from the 'pileStack' list
        _stackPrefab = pileStacks[Random.Range(0, pileStacks.Count())];

        for (int i = 0; i < GeneratedStackAmount; i++)
        {
            // Clone a pile of the selected stack
            var childStack = Instantiate(_stackPrefab, i * spacing * -Vector3.up,
                Quaternion.Euler(0, stepAngle * i, 0),
                transform);

            // Caches it to the {stacks} list
            _parentStacks.Add(childStack);
        }
    }

    public void AssignRandomMaterial()
    {
        // Assign material only if there is a stack
        if (_parentStacks.Count < 0)
            return;

        foreach (var childStack in _parentStacks)
        {
            _childrenCount = childStack.transform.childCount;

            var randomNumber = Random.Range(0, _childrenCount);

            for (int j = 0; j < _childrenCount; j++)
            {
                // Child of the parent pile stack
                var thisChild = childStack.transform.GetChild(j);

                var childStackRend = thisChild.GetComponent<MeshRenderer>();

                // Assign all child-stacks(children) a 'bad material' and tag
                childStackRend.sharedMaterial = colorBad;
                thisChild.tag = _badStackTag;

                // unequal values will skip this iteration
                if (j == randomNumber)
                    continue;

                // Re-assign child-stacks(children) a 'good material' and tag
                childStackRend.sharedMaterial = colorGood;
                thisChild.tag = _goodStackTag;
            }
        }
    }

    #region Editor Prototype
    // For testing, Can be removed.
    public void ClearStacks()
    {
        // Clears all stacks if available
        if (_parentStacks.Count < 0)
            return;

        _parentStacks.Clear();

        var children = transform.childCount;

        for (int i = children - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);
    }

    // For testing, to be removed later
    public void AssignOnlyGood()
    {
        if (_parentStacks.Count < 0)
            return;

        foreach (var childStack in _parentStacks)
        {
            _childrenCount = childStack.transform.childCount;

            for (int j = 0; j < _childrenCount; j++)
            {
                // Assign each child-stack a custom material
                var thisChild = childStack.transform.GetChild(j);

                thisChild.GetComponent<MeshRenderer>().sharedMaterial = colorGood;
                thisChild.tag = _goodStackTag;
            }
        }
    }
    #endregion

    public void RefreshCheckpoint()
    {
        // Generates a checkpoint at the last stack's position with a little offset
        // Works only if there is an available child stacks in its parent

        if (_parentStacks.Count == 0)
            return;

        // y-scale amount calculation of the transparent pipe
        var offsetSpacing = 2;
        var platformAmount = -(_parentStacks[_parentStacks.Count - 1].transform.position.y / offsetSpacing) + spacing;

        // Transparent Pipe positioning and scaling
        checkpointPipe.position = new Vector3(0, 1.5f, 0);

        // Transparent Pipe scaling
        var pipeScale = checkpointPipe.localScale;
        pipeScale.y = platformAmount + spacing;
        checkpointPipe.localScale = pipeScale;

        // Cylinder positioning
        checkpointCylinder.position = new Vector3(0, -(platformAmount * offsetSpacing) + spacing, 0);
    }

    public void AssignRandomMaterial2()
    {
        foreach (var item in _parentStacks)
        {
            // This corresponds to the number of child-stacks we have in a parent
            var goodStacks = new List<int>() { 0, 1, 2, 3, 4, 5 };
            var badStacks = new List<int>();

            // Number of child to assign a 'bad material' per parent, higher values indicate more children
            var randomNumber = Random.Range(0, 2);

            // example: random number generated(2); j < (2), then assign material to child(2) per parent
            for (int j = 0; j < randomNumber; j++)
            {
                var randomGoodStacks = Random.Range(0, goodStacks.Count);

                badStacks.Add(goodStacks[randomGoodStacks]);

                // Clear the generated number from the good-stacks list
                goodStacks.RemoveAt(randomGoodStacks);
            }

            var childStackRenderer = new List<Renderer>(item.GetComponentsInChildren<MeshRenderer>());

            for (int i = 0; i < childStackRenderer.Count; i++)
            {
                // If a number in the damagedList is found in the renderer list, set their material and tag
                if (badStacks.Contains(i))
                {
                    childStackRenderer[i].sharedMaterial = colorBad;
                    childStackRenderer[i].tag = _badStackTag;
                }
                else
                {
                    childStackRenderer[i].sharedMaterial = colorGood;
                    childStackRenderer[i].tag = _goodStackTag;
                }
            }
        }
    }
}
