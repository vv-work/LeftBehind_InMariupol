# ECS Unit Tests for Left Behind In Mariupol

This directory contains comprehensive unit tests for the ECS systems in the Left Behind In Mariupol project.

## Test Structure

### Base Classes
- **`ECSTestsFixture`**: Base class for all ECS tests that provides common setup including:
  - Test World creation and cleanup
  - EntityManager access
  - Physics system setup for collision testing
  - Helper methods for physics world updates and frame simulation

### System Tests
- **`FindTargetSystemTests`**: Tests for target acquisition system
  - Target finding within range
  - Faction-based targeting
  - Range-based filtering
  - Timer cooldown functionality

- **`UnitMoverSystemTests`**: Tests for unit movement system
  - Movement towards target positions
  - Stopping behavior near targets
  - Rotation towards movement direction
  - Multi-entity parallel processing
  - Edge cases (zero speed, etc.)

- **`SelectVisualSystemTests`**: Tests for selection visual feedback
  - Visual showing/hiding on selection events
  - Multiple entity selection handling
  - Component enable/disable behavior
  - Different scale values

- **`ResetEventsSystemTests`**: Tests for event flag cleanup
  - Event flag resetting after processing
  - Multi-entity event handling
  - Component state preservation
  - Disabled component handling

- **`IntegrationTests`**: End-to-end system interaction tests
  - Complete unit workflow testing
  - Multi-unit interaction scenarios
  - Time-based behavior testing

## Test Assembly Configuration

The tests use a dedicated assembly definition (`LeftBehindInMariupol.Tests.asmdef`) that references:
- Unity Test Framework
- Unity ECS packages
- Main project assembly
- Required physics and mathematics packages

## Running Tests

### Unity Test Runner
1. Open **Window > General > Test Runner**
2. Select **PlayMode** tab for runtime tests
3. Click **Run All** or select specific test classes
4. View results in the Test Runner window

### Command Line (CI/CD)
```bash
# Run all PlayMode tests
Unity -batchmode -runTests -testPlatform PlayMode -testResults results.xml

# Run specific test class
Unity -batchmode -runTests -testPlatform PlayMode -testFilter "Tests.FindTargetSystemTests"
```

## Test Features

### Physics Integration
- Tests use Unity Physics CollisionWorld for realistic collision detection
- Physics updates are handled through World.Update() for simplified testing
- Collider creation utilities for test entities

### Burst Compatibility
- All systems maintain Burst compilation during testing
- Tests verify system behavior with Burst-compiled parallel jobs

### Multi-Entity Testing
- Tests verify parallel processing capabilities
- Multiple entity scenarios ensure system scalability

### Edge Case Coverage
- Zero values and boundary conditions
- Disabled components and invalid states
- Empty queries and null references

## Best Practices

### Test Isolation
- Each test creates its own entities
- Automatic cleanup after each test
- No shared state between tests

### Performance Considerations
- Tests use minimal entity counts for speed
- Physics world updates only when necessary
- Efficient test data setup and teardown

### Maintainability
- Clear test naming following `Method_Scenario_ExpectedBehavior` pattern
- Comprehensive assertions with meaningful messages
- Helper methods for common test setup

## Adding New Tests

1. Create test class inheriting from `ECSTestsFixture`
2. Override `OnCreate()` to set up required systems
3. Use `OnSetUp()` and `OnTearDown()` for per-test setup/cleanup
4. Follow existing naming and structure patterns
5. Include edge cases and multi-entity scenarios

## Dependencies

- Unity 6 (6000.2.0f1)
- Unity Entities 1.3.14
- Unity Physics 1.3.14
- Unity Test Framework 1.5.1
- Unity Mathematics
- NUnit Framework

## Performance Testing

While these are unit tests focused on correctness, performance can be measured by:
- Running tests with Unity Profiler
- Using `[Performance]` attributes from Unity Performance Testing package
- Monitoring entity creation/destruction costs
- Measuring system update times

## Troubleshooting

### Common Issues
- **Physics not working**: Ensure `UpdatePhysicsWorld()` is called before physics-dependent tests
- **System not found**: Verify system is created in `OnCreate()` method
- **Entity cleanup**: Use proper teardown to avoid entity leaks between tests
- **Assembly references**: Check that test assembly references all required packages

### Debugging Tests
- Use Unity console for debug output during test runs
- Enable ECS debugging in Window > Entities for entity inspection
- Use breakpoints in test methods for step-by-step debugging