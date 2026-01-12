import React from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import LoginScreen from './screens/LoginScreen';
import TerminiScreen from './screens/TerminiScreen';
import NovTerminScreen from './screens/NovTerminScreen';
import OkvirniTerminiScreen from './screens/OkvirniTerminiScreen';
import AdminTerminiScreen from './screens/AdminTerminiScreen';
import AdminMenuScreen from './screens/AdminMenuScreen';
import AdminStrankeScreen from './screens/AdminStrankeScreen';
import AdminUserDetailScreen from './screens/AdminUserDetailScreen';
import AdminCreateTerminScreen from './screens/AdminCreateTerminScreen';
import SporocilaScreen from './screens/SporocilaScreen';
const Stack = createNativeStackNavigator();

export default function App() {
  return (
    <NavigationContainer>
      <Stack.Navigator
        initialRouteName="Login"
        screenOptions={{
          animation: 'none',
        }}
      >
        <Stack.Screen
          name="Login"
          component={LoginScreen}
          options={{ headerShown: false }}
        />
        <Stack.Screen
          name="Termini"
          component={TerminiScreen}
          options={{ title: 'Moje Prijave' }}
        />
        <Stack.Screen
          name="OkvirniTermini"
          component={OkvirniTerminiScreen}
          options={{ title: 'Razpoložljivi Termini' }}
        />
        <Stack.Screen
          name="NovTermin"
          component={NovTerminScreen}
          options={{ title: 'Nova Rezervacija' }}
        />
        <Stack.Screen
          name="AdminTermini"
          component={AdminTerminiScreen}
          options={{ title: '🎛️ Admin Panel' }}
        />
        <Stack.Screen
          name="AdminMenu"
          component={AdminMenuScreen}
          options={{ title: '🎛️ Admin Panel' }}
        />
        <Stack.Screen
          name="AdminStranke"
          component={AdminStrankeScreen}
          options={{ title: '👥 Stranke' }}
        />
        <Stack.Screen
          name="AdminUserDetail"
          component={AdminUserDetailScreen}
          options={{ title: 'Podrobnosti stranke' }}
        />
        <Stack.Screen
          name="AdminCreateTermin"
          component={AdminCreateTerminScreen}
          options={{ title: 'Dodaj termin' }}
        />
        <Stack.Screen
          name="Sporocila"
          component={SporocilaScreen}
          options={{ title: '💬 Sporočila' }}
        />
      </Stack.Navigator>
    </NavigationContainer>
  );
}