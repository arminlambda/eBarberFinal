import React from 'react';
import { View, Text, TouchableOpacity, StyleSheet } from 'react-native';

export default function AdminMenuScreen({ navigation }) {
  return (
    <View style={styles.container}>
      <Text style={styles.title}>🎛️ Admin Panel</Text>

      <TouchableOpacity
        style={styles.menuItem}
        onPress={() => navigation.navigate('AdminTermini')}
      >
        <Text style={styles.menuIcon}>📅</Text>
        <Text style={styles.menuText}>Okvirni termini</Text>
      </TouchableOpacity>

      <TouchableOpacity
        style={styles.menuItem}
        onPress={() => navigation.navigate('AdminStranke')}
      >
        <Text style={styles.menuIcon}>👥</Text>
        <Text style={styles.menuText}>Vse stranke</Text>
      </TouchableOpacity>

      <TouchableOpacity
        style={styles.menuItem}
        onPress={() => navigation.navigate('AdminCreateTermin')}
      >
        <Text style={styles.menuIcon}>➕</Text>
        <Text style={styles.menuText}>Dodaj okvirni termin</Text>
      </TouchableOpacity>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
    padding: 20,
  },
  title: {
    fontSize: 28,
    fontWeight: 'bold',
    marginBottom: 30,
    textAlign: 'center',
  },
  menuItem: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#fff',
    padding: 20,
    borderRadius: 12,
    marginBottom: 15,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  menuIcon: {
    fontSize: 32,
    marginRight: 15,
  },
  menuText: {
    fontSize: 18,
    fontWeight: '600',
    color: '#333',
  },
});